""" Main script for computing metrics for repo-level task experiments.

This script can be used to compute metrics for an approach on a repo. It manages internal
paths and calls on the suitable metric handler for the repo being evaluated.
"""

from pathlib import Path
import logging

from metrics import Ext1RepoMetricHandler, Ext2RepoMetricHandler, PythonMetricHandler, compute_blocks_metrics_for_repo, InputType, MetricType


_script_dir = Path(__file__).parent.absolute()
_data_dir = _script_dir.parent / "data"


CS_EXT_REPO_NAMES = {
    "ext1": [
        "codeplan",
        "repair",
    ],
    "ext2": [
        "codeplan",
        "codeplan_iter_2",
        "repair",
    ],
}

PYTHON_EXT_REPO_NAMES = {
    "t1": [
        "codeplan",
        "repair",
        "repair_strict",
        "coeditor_codeplan",
        "coeditor_repair",
        "coeditor_repair_strict",
        "codeplan_no_context",
    ],
    "t2": [
        "codeplan",
        "repair",
        "repair_strict",
        "coeditor_codeplan",
        "coeditor_repair",
        "coeditor_repair_strict",
        "codeplan_no_context",
    ],
    "t3": [
        "codeplan",
        "repair",
        "repair_strict",
        "coeditor_codeplan",
        "coeditor_repair",
        "coeditor_repair_strict",
        "codeplan_no_context",
    ],
}

REPO_NAMES = list(CS_EXT_REPO_NAMES.keys()) + list(PYTHON_EXT_REPO_NAMES.keys())



class RepoOrApproachNotFound(Exception):
    def __init__(self, message, dir_path: Path | None = None):
        super().__init__(message)
        self.dir_path = dir_path


def get_repo_approach_dir(
    repo_name: str,
    approach_name: str,
) -> tuple[Path, Path]:
    repo_data_dir = _data_dir / repo_name
    if not repo_data_dir.exists():
        raise RepoOrApproachNotFound(
            f"Could not find repo {repo_name} at {repo_data_dir}",
            dir_path=repo_data_dir,
        )
    
    approach_data_dir = repo_data_dir / "pred" / approach_name
    if not approach_data_dir.exists():
        raise RepoOrApproachNotFound(
            f"Could not find approach {approach_name} for repo {repo_name} at {approach_data_dir}",
            dir_path=approach_data_dir,
        )
    
    return repo_data_dir, approach_data_dir


def eval_repo_approach(
    repo_name: str,
    approach_name: str,
    metrics: list[tuple[InputType, MetricType]] = [(InputType.FILE_DIFF, MetricType.BLEU)]
) -> dict:
    
    repo_data_dir, approach_data_dir = get_repo_approach_dir(
        repo_name=repo_name,
        approach_name=approach_name,
    )

    approach_repo_dir = approach_data_dir / "repo"
    approach_blocks_data_dir = approach_data_dir / "blocks"
    source_data_dir = repo_data_dir / "source"
    target_data_dir = repo_data_dir / "target"
    
    match repo_name:
        case "ext1":
            metric_handler = Ext1RepoMetricHandler(
                source_dir=source_data_dir,
                target_dir=target_data_dir,
                pred_dir=approach_repo_dir,
            )
        case "ext2":
            metric_handler = Ext2RepoMetricHandler(
                source_dir=source_data_dir,
                target_dir=target_data_dir,
                pred_dir=approach_repo_dir,
            )
        case "t1" | "t2" | "t3":
            metric_handler = PythonMetricHandler(
                source_dir=source_data_dir,
                target_dir=target_data_dir,
                pred_dir=approach_repo_dir,
            )
        case _:
            raise ValueError(f"Invalid repo name: {repo_name}")


    text_metrics = metric_handler.compute_text_metrics(
        metrics=metrics,
        num_process=1,
    )

    file_wise_diff_bleu = None
    file_wise_ld = None
    for (input_type, metric_type), results in text_metrics.items():
        match (input_type, metric_type):
            case (InputType.FILE_DIFF, MetricType.BLEU):
                file_wise_diff_bleu = results
            case (InputType.FILE, MetricType.LEVENSHTEIN):
                file_wise_ld = results

    
    if file_wise_diff_bleu is None:
        logging.warning(f"File-wise diff bleu metric not computed for {repo_data_dir}.")
        avg_diff_bleu = None
    else:
        avg_diff_bleu = sum(file_wise_diff_bleu.values()) / len(file_wise_diff_bleu)
    
    if file_wise_ld is None:
        logging.warning(f"File-wise levenshtein metric not computed for {repo_data_dir}.")
        total_ld = None
    else:
        total_ld = sum(file_wise_ld.values())


    matched_blocks_file = approach_blocks_data_dir / "matched.txt"
    missed_blocks_file = approach_blocks_data_dir / "missed.txt"
    spurious_blocks_file = approach_blocks_data_dir / "spurious.txt"

    blocks_metrics = compute_blocks_metrics_for_repo(
        matched_blocks_path=matched_blocks_file,
        missed_blocks_path=missed_blocks_file,
        spurious_blocks_path=spurious_blocks_file,
    )

    result_metrics = {
        "summary": {
            "avg_diff_bleu": avg_diff_bleu,
            "total_ld": total_ld,
            **blocks_metrics,
        },
        "full": {
            "text": [{"input_type": _input_type.value, "metric_type": _metric_type.value, "results": _metrics} for (_input_type, _metric_type), _metrics in text_metrics.items()],
            "blocks": blocks_metrics,
        }
    }

    return result_metrics

    

if __name__ == "__main__":
    from argparse import ArgumentParser
    import json

    parser = ArgumentParser()
    parser.add_argument("--repo", "-r", type=str, choices=REPO_NAMES, help="Repo name to compute metrics for.")
    parser.add_argument("--approach", "-a", type=str, help="Approach name to compute metrics for.")
    parser.add_argument("--all", action="store_true", help="Compute metrics for all approaches on all repos.")
    parser.add_argument("--levenstein", action="store_true", help="Compute levenstein distance metric. Note that this may take a really long time")
    parser.add_argument("--save_default", action="store_true", help="Save metrics to default location.")
    parser.add_argument("--save_path", "-s", type=str, default=None, help="Path to save json with all computed metrics to.")
    parser.add_argument("--verbose", "-v", action="store_true", help="Enable verbose (info) logging.")
    parser.add_argument("--debug", "-d", action="store_true", help="Enable debug logging.")
    args = parser.parse_args()

    if args.save_default and args.save_path is not None:
        raise ValueError("Cannot specify both --save_default and --save_path.")

    if args.verbose:
        logging.basicConfig(level=logging.INFO)
    if args.debug:
        logging.basicConfig(level=logging.DEBUG)

    if args.save_path: 
        save_path = Path(args.save_path)
        if not save_path.parent.exists():
            logging.info(f"Creating parent directories for save path: {save_path.parent}")
            save_path.parent.mkdir(parents=True)
        else:
            if save_path.exists():
                raise ValueError(f"File already exists at save path: {save_path}")
    else:
        logging.info("No save path provided.")
        save_path = None

    metrics_and_input_types: list[tuple[InputType, MetricType]] = [(InputType.FILE_DIFF, MetricType.BLEU)]
    if args.levenstein:
        metrics_and_input_types.append((InputType.FILE, MetricType.LEVENSHTEIN))

    if args.all:
        logging.info("Evaluating all approaches on all repos.")
        metrics = []
        for repo_name in REPO_NAMES:
            approach_names = CS_EXT_REPO_NAMES.get(repo_name, []) + PYTHON_EXT_REPO_NAMES.get(repo_name, [])
            
            for approach_name in approach_names:
                try:
                    logging.info(f"Evaluating approach {approach_name} on repo {repo_name}")
                    _metrics = eval_repo_approach(
                        repo_name=repo_name,
                        approach_name=approach_name,
                        metrics=metrics_and_input_types,
                    )
                    print(f"Metrics for approach {approach_name} on repo {repo_name}:\n{_metrics['summary']}\n")
                    metrics.append({
                        "repo": repo_name,
                        "approach": approach_name, 
                        "metrics": _metrics,
                    })

                except RepoOrApproachNotFound as e:
                    logging.warning(f"Skipping approach {approach_name} for repo {repo_name} as no data found at {e.dir_path}")
                    continue

                if args.save_default:
                    _, approach_data_dir = get_repo_approach_dir(repo_name, approach_name)
                    default_save_path = approach_data_dir / "metrics.json"
                    default_save_path.write_text(json.dumps(_metrics, indent=4))
                    logging.info(f"Saved metrics for approach {approach_name} for repo {repo_name} to {default_save_path.absolute()}")

    else:
        logging.info(f"Evaluating approach {args.approach} on repo {args.repo}")
        metrics = eval_repo_approach(
            repo_name=args.repo,
            approach_name=args.approach,
            metrics=metrics_and_input_types,
        )
        print(f"Metrics for approach {args.approach} on repo {args.repo}:\n{metrics['summary']}\n")

        if args.save_default:
            _, approach_data_dir = get_repo_approach_dir(args.repo, args.approach)
            default_save_path = approach_data_dir / "metrics.json"
            default_save_path.write_text(json.dumps(metrics, indent=4))
            logging.info(f"Saved metrics for approach {args.approach} for repo {args.repo} to {default_save_path.absolute()}")

    if save_path:
        save_path.write_text(json.dumps(metrics, indent=4))
        logging.info(f"Saved metrics to {save_path.absolute()}")
