# Assumptions

This file lists the decisions that were not clearly defined in the spec, or that we chose ourselves. Requirements from the spec itself are not repeated here.

## Login and identity

1) There is no login or password. The guardian enters a 17-digit Student ID, which is stored in the localStorage.
2) We do not collect the student's name, phone number, or any other personal details. The `students` table only stores `student_birth_id` which basically 17 digits in BDand `created_at`.
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

1) The spec mentions a single repository, but the backend and frontend are kept in two separate repositories for my flexibility. Docker Compose builds both from the sibling folders.