# Assumptions

This file lists the decisions that were not clearly defined in the spec, or that we chose ourselves. Requirements from the spec itself are not repeated here.

## Login and identity

1) There is no login or password. The guardian enters a 17-digit Student ID, which is stored in the localStorage.
2) We do not collect the student's name, phone number, or any other personal details. The `students` table only stores `student_birth_id` which is the 17-digit birth registration number, and `created_at`.
3) The admin page (`/admin`) and the guardian page (`/`) are separate routes. The admin page has no authentication.

## Classes

1) Class names may repeat (for example, two classes both named "Class 6 - Day Shift"). The unique identifier for a class is `class_id`.

## Payment

1) There is no real payment gateway. Clicking "Yes" in the confirmation dialog is treated as the fee being paid.

## Errors and edge cases

1) If a student tries to confirm or cancel a hold that belongs to another student, the API returns `HOLD_NOT_FOUND`.
2) If a student tries to confirm a hold that has already expired, the API returns `HOLD_EXPIRED`.

## Time

1) All timestamps are stored in UTC in the database. The frontend displays them in local time.
2) A background service marks expired holds every 5 seconds. The frontend countdown is only for display; the server's expiry time is the source of truth.

## Repository layout

1) The backend and frontend were first built in two separate repositories. They were merged into this single repository (with `git subtree`, so the history of both is kept), with `backend/` and `frontend/` folders and `docker-compose.yml` at the root.

## Two backend processes on one database (bonus)

1) The seat limit does not rely on any in-memory lock. Every apply runs in a database transaction that first locks the class row with `SELECT ... FOR UPDATE`, then counts held + confirmed seats and inserts the hold. PostgreSQL makes any other transaction for the same class wait on that row lock, even if it comes from a different backend process on a different port, so `held + confirmed` can never go over `totalSeats`.
2) Confirm and cancel also lock the hold row with `SELECT ... FOR UPDATE`, so a double confirm from two processes still counts only once. The partial unique index `holds_one_live_per_student_class_uq` is a database-level backstop for the one-application rule.