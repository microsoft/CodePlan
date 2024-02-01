import base64
import logging
import tempfile
from pathlib import Path
import os
import subprocess


def get_modified_files(repo_dir, pre_sha, post_sha):
    logging.info(f"Getting list of modified files for repo at {repo_dir} between commit shas {pre_sha} and {post_sha} ...")

    diff_command = f'git --no-index --no-pager diff --name-only "{pre_sha}" "{post_sha}" --diff-filter=M'
    result = subprocess.run(
        diff_command,
        shell=True,
        text=True,
        cwd=repo_dir,
        capture_output=True,
    )
    return result.stdout.splitlines()


GIT_ATTRIBUTES_FOR_LANG_SPECIFIC_DIFF = """*.cs\tdiff=csharp
*.py\tdiff=python"""


def get_diff(pre, post, repo_dir = None, file_path = None, file_paths = None, function_context: bool = False, diff_command: str | None = None):
    if file_path is not None and file_paths is not None:
        raise ValueError("Only one of file_path and file_paths should be specified.")
    
    if repo_dir is not None:
        _log_str = f"Getting diff in repo at {repo_dir} between commit shas {pre} and {post}"
    else:
        _log_str = f"Getting diff between {pre} and {post}"
    
    if file_path:
        _log_str += f" for file {file_path}"
    elif file_paths:
        _log_str += f" for {len(file_paths)} files"

    if function_context:
        _log_str += " with function context"
    _log_str += " ..."
    logging.debug(_log_str)


    temp_dir = tempfile.TemporaryDirectory(ignore_cleanup_errors=True)
    attributes_file_path = Path(temp_dir.name) / "git/attributes"
    attributes_file_path.parent.mkdir(parents=True)
    attributes_file_path.write_text(GIT_ATTRIBUTES_FOR_LANG_SPECIFIC_DIFF)
    os.environ["XDG_CONFIG_HOME"] = str(temp_dir.name)

    if diff_command is None:
        diff_command = f"git --no-pager diff --no-index "

    if function_context:
        diff_command += "-U0 --function-context "

    diff_command += f'"{pre}" "{post}"'

    if file_path:
        diff_command += f' -- "{file_path}"'
    elif file_paths:
        paths_list_str = " ".join([str(p) for p in file_paths])
        diff_command += f' -- {paths_list_str}'

    logging.debug(diff_command)
    if repo_dir is not None:
        result =  subprocess.run(
            diff_command,
            shell=True,
            text=True,
            cwd=repo_dir,
            capture_output=True,
            encoding="utf-8",
        )
    else:
        result =  subprocess.run(
            diff_command,
            shell=True,
            text=True,
            capture_output=True,
            encoding="utf-8",
        )

    temp_dir.cleanup()
    os.environ.pop("XDG_CONFIG_HOME", None)

    if result.returncode != 0 and result.stderr != "" and not result.stderr.startswith("warning:"):
        raise RuntimeError(f"Failed to get diff in repo at {repo_dir} between {pre} and {post} using the following diff command:\n{diff_command}\n\n resulting in the following error:\n{result.stderr}")

    return result.stdout


def clone_repo(repo_url, repo_dir, pat = None, username = None):
    if pat is not None:
        username = username if username is not None else ""
        b64_auth_token = base64.b64encode(bytes(f"{username}:{pat.strip()}", 'utf-8')).decode('utf-8')
        clone_command = f'git -c http.extraHeader="Authorization: Basic {b64_auth_token}" clone {repo_url} {repo_dir}'
    else:
        clone_command = f'git clone {repo_url} {repo_dir}'

    logging.info(f"Cloning repo from URL {repo_url} into directory {repo_dir} ...")
    result = subprocess.run(clone_command, shell=True, stdout=subprocess.DEVNULL, stderr=subprocess.DEVNULL)
    if result.returncode != 0:
        raise RuntimeError(f"Failed to clone repo from URL {repo_url} into directory {repo_dir}.")


def checkout_repo(repo_dir, commit):
    logging.info(f"Checking out repo at {repo_dir} at {commit} ...")
    result = subprocess.run(f'git checkout {commit}', shell=True, cwd=repo_dir, stdout=subprocess.DEVNULL, stderr=subprocess.DEVNULL)
    if result.returncode != 0:
        raise RuntimeError(f"Failed to checkout repo at {repo_dir} at {commit}.")


def create_new_branch(repo_dir, new_branch_name, old_commit):
    logging.info(f"Creating new branch {new_branch_name} from {old_commit} in repo at {repo_dir} ...")
    result = subprocess.run(f'git checkout -b {new_branch_name} {old_commit}', shell=True, cwd=repo_dir, stdout=subprocess.DEVNULL, stderr=subprocess.DEVNULL)
    if result.returncode != 0:
        raise RuntimeError(f"Failed to create new branch {new_branch_name} from {old_commit} in repo at {repo_dir}.")


def commit_changes(repo_dir, commit_message):
    logging.info(f"Committing changes in repo at {repo_dir} with message {commit_message} ...")
    result = subprocess.run(f'git commit -m "{commit_message}"', shell=True, cwd=repo_dir, stdout=subprocess.DEVNULL, stderr=subprocess.DEVNULL)
    if result.returncode != 0:
        raise RuntimeError(f"Failed to commit changes in repo at {repo_dir} with message {commit_message}.")


def stage_and_commit_changes(repo_dir, commit_message, files_to_stage = None):
    if files_to_stage is None:
        logging.info(f"Committing all changes in repo at {repo_dir} with message {commit_message} ...")
        result = subprocess.run(f'git commit -am "{commit_message}"', shell=True, cwd=repo_dir, stdout=subprocess.DEVNULL, stderr=subprocess.DEVNULL)
        if result.returncode != 0:
            raise RuntimeError(f"Failed to commit changes in repo at {repo_dir} with message {commit_message}.")
    else:
        logging.info(f"Committing changes in repo at {repo_dir} with message {commit_message} for files {files_to_stage} ...")
        results = subprocess.run(f'git commit -m "{commit_message}" {" ".join(files_to_stage)}', shell=True, cwd=repo_dir, stdout=subprocess.DEVNULL, stderr=subprocess.DEVNULL)
        if results.returncode != 0:
            raise RuntimeError(f"Failed to commit changes in repo at {repo_dir} with message {commit_message} for files {files_to_stage}.")


def push_changes(repo_dir, remote_name="", branch_name=""):
    logging.info(f"Pushing changes in repo at {repo_dir} to remote {remote_name} branch {branch_name} ...")
    result = subprocess.run(f'git push {remote_name} {branch_name}', shell=True, cwd=repo_dir, stdout=subprocess.DEVNULL, stderr=subprocess.DEVNULL)
    if result.returncode != 0:
        raise RuntimeError(f"Failed to push changes in repo at {repo_dir} to remote {remote_name} branch {branch_name}.")

def set_remote_url(repo_dir, remote_name, remote_url):
    logging.info(f"Setting remote URL for remote {remote_name} to {remote_url} for repo at {repo_dir} ...")
    results = subprocess.run(f'git remote set-url {remote_name} {remote_url}', shell=True, cwd=repo_dir, stdout=subprocess.DEVNULL, stderr=subprocess.DEVNULL)
    if results.returncode != 0:
        raise RuntimeError(f"Failed to set remote URL for remote {remote_name} to {remote_url} for repo at {repo_dir}.")

def get_current_branch(repo_dir):
    logging.info(f"Getting current branch for repo at {repo_dir} ...")
    result = subprocess.run(f'git branch --show-current', shell=True, text=True, capture_output=True, cwd=repo_dir)
    if result.returncode != 0:
        raise RuntimeError(f"Failed to get current branch for repo at {repo_dir}.")
    return result.stdout
