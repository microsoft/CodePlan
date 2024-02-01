# Replication Package

For the paper titled "CodePlan: Repository-level Coding using LLMs and Planning", accepted FSE 2024.


## Contents

This package contains the following 

- `data/`: Benchmark of repository edits along with output of our approach and baselines.
- `scripts/`: Script to compute key metrics presented in the paper.

## Data

Data for each repository edit is presented in different directory with the names used in the paper.
Each these contains three sub-directories - (1) `source`: repository before the edit, `target`: repository after the ground truth edit and `pred`: output of our approach and baselines.
`pred` contains different sub-directories for each approach, where each subdirectory contains `repo`: state of the repository after edits, `blocks`: matched, missed and spurious blocks in the output, `metrics.json`: all metrics computed for the repo, `diff.html`: pretty printed textual diff of the output and source (along with diff between source and target for comparison).

```
data
| -- t1
      | -- source
      | -- target
      | -- pred
            | -- codeplan
                  | -- repo
                  | -- blocks
                  | -- metrics.json
                  | -- diff.html
            | -- repair
                  | -- repo
                  | -- blocks
                  | -- diff.html
            ...
...
```

### Quick Start!

To inspect the metrics for a particular repository edit, just navigate to its directory and open `metrics.json`.
To inspect the code change, you can similarly open `diff.html`


## Replicating results

### Code Setup

The scripts require `python>=3.11` along with the following packages - 

```
tqdm
evaluate
textdistance
```

### Computing metrics

The main script for computing metrics is `scripts/eval.py` with the following options - 

```
usage: eval.py [-h] [--repo {ext1,ext2,t1,t2,t3}]       
               [--approach APPROACH] [--all]
               [--levenstein] [--save_default]
               [--save_path SAVE_PATH] [--verbose]      
               [--debug]

options:
  -h, --help            show this help message and exit 
  --repo {ext1,ext2,t1,t2,t3}, -r {ext1,ext2,t1,t2,t3}  
                        Repo name to compute metrics    
                        for.
  --approach APPROACH, -a APPROACH
                        Approach name to compute metrics
                        for.
  --all                 Compute metrics for all
                        approaches on all repos.        
  --levenstein          Compute levenstein distance     
                        metric. Note that this may take 
                        a really long time
  --save_default        Save metrics to default
                        location.
  --save_path SAVE_PATH, -s SAVE_PATH
                        Path to save json with all      
                        computed metrics to.
  --verbose, -v         Enable verbose (info) logging.    
  --debug, -d           Enable debug logging.
```

Note that the names of the approach must match one of the sub-directories present within the `pred` directory for the repo being evaluated.

For example to compute metrics for approach `codeplan` on repo `t1` and save the results to `t1_codeplan_stats.json` the following command can be used -

```
python scripts/eval.py --repo t1 --approach codeplan --save_path t1_codeplan_stats.json
```

This will compute text metrics (DiffBLEU, Levenshtein Distance) and block metrics (matched, missing spurious), print out a summary corresponding to a row in Table 3 of the paper and store detailed file-wise metrics to the provided path.
