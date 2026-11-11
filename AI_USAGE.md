# AI Usage

This project was built with help from Claude.

## What AI was used for

 1) Planning:Breaking the spec into features and deciding the order of work.
 2) Database design: Proposing the schema for students, classes, and holds, and discussing the trade-off of storing only the student ID.
 3) Backend code: Generating the initial structure of the clean architecture layers (Domain, Application, Infrastructure, API), the EF Core configuration, the seat-hold logic, and the concurrency tests.
 3) Frontend code: Generating the React components for the guardian page and the admin page.
 4) Docker: Writing the Dockerfiles and `docker-compose.yml`.
 5) Debugging: Reading error messages from the build, migrations, CORS errors, and Git output.

## What I did myself

 1) Chose the requirements that were left open, such as: no login, no name collection, duplicate class names allowed, separate admin and guardian routes.
 2) Ran every command locally and checked the output before moving on.
 3) Tested the seat-hold behaviour with the concurrency tests and by using the UI.
 4) Built the backend and frontend in two repositories at first, then merged them into one repository with `backend/` and `frontend/` folders as the spec requires.

## Mistakes the AI made

 1) Compose path: The AI gave two different frontend build paths (`./frontend` and `../school-admission-system-frontend`) and did not clearly explain the difference, which caused a build error.
 2) Git workflow: The AI first suggested merging a feature branch and pushing `main` directly, which skipped the pull request step. It also suggested `git reset --hard origin/main`, which discarded a local change I had not committed yet. The change was later recovered from the reflog.
 3) Frontend folder: The AI first suggested adding a `frontend/` folder to the backend repo with `git subtree`, then changed direction. This caused confusion about the repository layout.
 4) Duplicate files: The AI did not notice a duplicate solution file (`school.slnx` and `School Admission System.slnx`) for a while.
 5) Assumptions file: The first draft of `ASSUMPTIONS.md` mixed spec requirements with real assumptions. I corrected it after review.

## How I checked the AI's output

 1) Built and ran the backend with Docker and checked `/health`, class creation, and the hold flow.
 2) Ran the concurrency tests (50 parallel requests for 10 seats; 20 parallel requests from the same student for one hold).
 3) Read Git output after every push, merge, and pull, and checked the commit history.
 4) Checked the frontend in the browser and used the DevTools console to find CORS and connection errors.