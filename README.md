# Alumni - Graduate Tracking & Networking System

An alumni information and networking platform designed to connect graduates, track their career trajectories, manage alumni verification, and facilitate communication between graduates, university departments, and corporate partners.

---

## 1. What the System Is

**Alumni** is a back-office and graduate relationship management system. It provides an official database for verified university graduates, tracks their employment records over time, and fosters ongoing collaboration between alumni, faculties, and corporate recruiters.

---

## 2. What It Has to Do

* **Alumni Verification & Onboarding:**
  * Facilitates the approval of new graduate registrations by university administration to prevent unauthorized or fake profiles.
* **Career & Employment Tracking:**
  * Stores and tracks graduates' career histories, current employment status, roles, companies, and industry sectors.
* **Advanced Querying & Filtering:**
  * Enables multi-parameter searches (e.g., *"Graduates of class 2024 working at Company X"*).
* **Department & Company Auditing:**
  * Audits departmental alumni statistics and manages institutional partnerships with hiring companies.
* **Back-Office Administration:**
  * Provides administrators with a secure portal to review, approve, and oversee records with minimal overhead.

---

## 3. Technology Stack & Architectural Decisions

Following the project requirements and fast delivery timeline, the architecture is built on **Python (Django) + PostgreSQL**.

### Backend: Python (Django)
* **Why chosen? (Pitch):**
  * The system requires an administration panel from day one to verify alumni, manage departments, and audit corporate partners. 
  * Django's built-in **Admin Panel** delivers a production-ready back-office out of the box, eliminating the need to build a custom management UI from scratch and allowing full focus on core business logic.
* **Trade-off / Known Limitation:**
  * Django's "batteries-included", monolithic design can introduce unnecessary weight. Departing from framework conventions is challenging, and its runtime performance is lower compared to compiled languages (such as C# or Go).

### Database: PostgreSQL
* **Why chosen? (Pitch):**
  * Provides rock-solid, native integration with Django ORM.
  * Optimized indexing capabilities (B-Tree/GIN) ensure fast response times for complex career-history and multi-criteria graduate queries (e.g., filtering by graduation year and current employer).
* **Trade-off / Known Limitation:**
  * Lacks the flexible, schema-free nature of NoSQL databases; schema updates require strict migration processes for every model modification.

### Containerization: Docker & Docker Compose
* All services (application server and database) are containerized to guarantee environment parity and zero-setup deployment.

---

## 4. How to Run

> **The Golden Rule:** The system must come up with a single command: `docker compose up`

### 1. Clone the Repository
```bash
git clone https://github.com/Sheva0710/Alumni.git
cd Alumni
