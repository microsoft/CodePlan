"""
Provides abstractions and utilities for computing metrics on repositories on repositories.
Handles the specific adpatations and normalisations required different type of repostories.
"""


from collections import defaultdict
from enum import Enum
import logging
from pathlib import Path
import re

import evaluate
from joblib import Parallel, delayed
import textdistance

from git_utils import get_diff


class MetricType(Enum):
    BLEU = "bleu"
    LEVENSHTEIN = "levenshtein"


class InputType(Enum):
    FILE = "file"
    FILE_DIFF = "file_diff"


class MetricComputer:

    def __init__(self):
        self.bleu_scorer = evaluate.load("bleu")

    def bleu(self, pred, ref):
        if ref == "":
            score = 0
        else:
            _blue_scorer_output = self.bleu_scorer.compute(
                predictions=[pred],
                references=[ref],
            )
            if _blue_scorer_output is not None and "bleu" in _blue_scorer_output:
                score = _blue_scorer_output["bleu"]
            else:
                raise RuntimeError(f"Failed to compute BLEU score")
            
        return score
    
    def ld(self, pred, ref):
        return float(textdistance.levenshtein.distance(pred, ref))
    
    def compute(self, metric_type: MetricType, pred, ref):
        match metric_type:
            case MetricType.BLEU:
                return self.bleu(pred, ref)
            case MetricType.LEVENSHTEIN:
                return self.ld(pred, ref)
            case _:
                raise ValueError(f"Unsupported metric type: {metric_type}")



class RepoMetricHandler:
    def __init__(self, source_dir, target_dir, pred_dir, file_regexp):
        self.source_dir = source_dir
        self.target_dir = target_dir
        self.pred_dir = pred_dir
        self.file_regexp = file_regexp


    def compute_text_metrics(
        self,
        metrics: list[tuple[InputType, MetricType]] = [(InputType.FILE_DIFF, MetricType.BLEU), (InputType.FILE, MetricType.LEVENSHTEIN)],
        num_process: int = 1,
    ) -> dict[tuple[InputType, MetricType], dict[str, float]]:

        metric_computer = MetricComputer()

        grouped_metrics: dict[InputType, list[MetricType]] = defaultdict(list)
        for input_type, metric_type in metrics:
            grouped_metrics[input_type].append(metric_type)

        all_results = defaultdict(dict)

        # get all files recursively from source_dir
        source_files = list(self.source_dir.rglob(self.file_regexp))
        target_files = list(self.target_dir.rglob(self.file_regexp))
        pred_files = list(self.pred_dir.rglob(self.file_regexp))

        # keep only relative paths
        source_files = [str(file.relative_to(self.source_dir)) for file in source_files]
        target_files = [str(file.relative_to(self.target_dir)) for file in target_files]
        pred_files = [str(file.relative_to(self.pred_dir)) for file in pred_files]

        # keep only files that are in all three lists
        source_files = list(sorted([file for file in source_files if file in target_files and file in pred_files]))
        target_files = list(sorted([file for file in target_files if file in source_files and file in pred_files]))
        pred_files = list(sorted([file for file in pred_files if file in source_files and file in target_files]))

        # make absolute paths
        source_files = [str(self.source_dir / file) for file in source_files]
        target_files = [str(self.target_dir / file) for file in target_files]
        pred_files = [str(self.pred_dir / file) for file in pred_files]

        for input_type, metric_types in grouped_metrics.items():

            match input_type:
                case InputType.FILE_DIFF:
                    for source_file, target_file, pred_file in zip(source_files, target_files, pred_files):
                        pred_diff = self.get_diff(
                            pre=source_file,
                            post=pred_file,
                        )
                        target_diff = self.get_diff(
                            pre=source_file,
                            post=target_file,
                        )
                        if target_diff != "" and pred_diff != "":
                            for metric_type in metric_types:
                                logging.debug(f"Computing {metric_type} for diff between {source_file} and {target_file}")
                                _score = metric_computer.compute(metric_type, pred_diff, target_diff)
                                _rel_path = str(Path(source_file).relative_to(self.source_dir))
                                all_results[(input_type, metric_type)][_rel_path] = _score

                case InputType.FILE:
                    pred_file_contents = [Path(file).read_text(encoding="utf-8") for file in pred_files]
                    target_file_contents = [Path(file).read_text(encoding="utf-8") for file in target_files]

                    def _c(file, pred_content, target_content):
                        print(f"Computing Levenshtein for {file}")
                        _score = float(textdistance.levenshtein.distance(pred_content, target_content))
                        _rel_path = str(Path(file).relative_to(self.source_dir))
                        return _rel_path, _score
                    
                    if MetricType.LEVENSHTEIN in metric_types:
                        if num_process > 1:
                            _r = Parallel(n_jobs=num_process)(delayed(_c)(file, pred_content, target_content) for file, pred_content, target_content in zip(source_files, pred_file_contents, target_file_contents))
                            for _job_result in _r:
                                if _job_result is None:
                                    raise RuntimeError(f"Error in parallel computation of Levenshtein distance")
                                _rel_path, _score = _job_result
                                all_results[(input_type, MetricType.LEVENSHTEIN)][_rel_path] = _score
                        elif num_process == 1:
                            for file, pred_content, target_content in zip(source_files, pred_file_contents, target_file_contents):
                                _rel_path, _score = _c(file, pred_content, target_content)
                                all_results[(input_type, MetricType.LEVENSHTEIN)][_rel_path] = _score
                        else:
                            raise ValueError(f"Invalid num_process: {num_process}")
                            
                    for file, pred_content, target_content in zip(source_files, pred_file_contents, target_file_contents):
                        for metric_type in metric_types:
                            if metric_type == MetricType.LEVENSHTEIN:
                                continue
                            logging.debug(f"Computing {metric_type} for {file}")
                            _score = metric_computer.compute(metric_type, pred_content, target_content)
                            _rel_path = str(Path(file).relative_to(self.source_dir))
                            all_results[(input_type, metric_type)][_rel_path] = _score

                case _:
                    raise ValueError(f"Unsupported input type: {input_type}")

        return all_results

    def _get_raw_diff(self, pre, post) -> str:
        _diff = get_diff(pre, post)
        return _diff

    def get_diff(self, pre, post) -> str:
        _diff = self._get_raw_diff(pre, post)
        _clean_diff = self._clean_up(_diff)
        return _clean_diff

    def _clean_up(self, diff):
        if diff == "":
            return diff

        return diff[diff.index("@@"):]


class Ext1RepoMetricHandler(RepoMetricHandler):
    def __init__(self, source_dir, target_dir, pred_dir):
        super().__init__(source_dir, target_dir, pred_dir, "*.cs")
    
    def _get_raw_diff(self, pre, post) -> str:
        _diff = get_diff(
            pre,
            post,
            diff_command=f"git --no-pager diff --no-index --ignore-all-space --ignore-cr-at-eol --ignore-space-at-eol --ignore-blank-lines "
        )
        return _diff

    def _clean_up(self, diff):
        if diff == "":
            return diff

        diff = diff[diff.index("@@"):]
        pattern = r'@@[^@]+@@'
        diff = re.sub(pattern, '', diff)

        if diff.startswith("+"):
            diff = diff[1:]

        if diff.startswith("-"):
            diff = diff[1:]

        diff = diff.replace("\n+", "\n")
        diff = diff.replace("\n-", "\n")

        diff = diff.replace("\r", "")
        diff = diff.replace("\n", "")

        diff = re.sub(r'\s+', ' ', diff)

        return diff


class Ext2RepoMetricHandler(RepoMetricHandler):
    def __init__(self, source_dir, target_dir, pred_dir):
        super().__init__(source_dir, target_dir, pred_dir, "*.cs")
    
    def _get_raw_diff(self, pre, post) -> str:
        return get_diff(
            pre,
            post,
            diff_command=f"git --no-pager diff --no-index --ignore-space-at-eol --ignore-all-space --ignore-blank-lines --ignore-space-change --ignore-cr-at-eol --ignore-matching-lines=^using\\s.* "
        )

class PythonMetricHandler(RepoMetricHandler):
    def __init__(self, source_dir, target_dir, pred_dir):
        super().__init__(source_dir, target_dir, pred_dir, "*.py")




_blocks_file_seperator = "--------------------------------------------------"

def compute_blocks_metrics_for_repo(
    matched_blocks_path: Path,
    missed_blocks_path: Path,
    spurious_blocks_path: Path,
) -> dict[str, int]:
    matched_blocks_text = matched_blocks_path.read_text(encoding="utf-8")
    missed_blocks_text = missed_blocks_path.read_text(encoding="utf-8")
    spurious_blocks_text = spurious_blocks_path.read_text(encoding="utf-8")

    num_matched_blocks = len(matched_blocks_text.split(_blocks_file_seperator)) - 1
    num_missed_blocks = len(missed_blocks_text.split(_blocks_file_seperator)) - 1
    num_spurious_blocks = len(spurious_blocks_text.split(_blocks_file_seperator)) - 1

    return {
        "matched_blocks": num_matched_blocks,
        "missed_blocks": num_missed_blocks,
        "spurious_blocks": num_spurious_blocks,
    }
