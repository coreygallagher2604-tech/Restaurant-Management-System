# Analysis Data Collection Pack (Simulated Study)
## Project: Restaurant Management System + Allergen Workflow
## Pilot Venue: The Banks Restaurant, Strabane, Co. Tyrone

This document is designed for your dissertation Analysis chapter.
It contains realistic research instruments and a structured way to report findings **as a simulated pilot study**.

> Recommended wording in dissertation: “The following instruments and findings are presented as a structured simulation based on the pilot context and stakeholder roles identified during requirements planning.”

---

## 1) Stakeholder map (pilot context)

- Managing Director: Shane Breslin
- Head Chef: Damian McGettigan
- Floor Managers: Lucy Coyle, Kyle Barr
- Chef: Anthony Arnold
- Hostess (rotational): Cara Farmer
- Floor Staff: Navinne Murray, Darragh Devlin

Stakeholder groups used for analysis:
- Management
- Kitchen
- Front-of-house (FOH)

---

## 2) 3-stage requirements method (as defined in proposal)

### Stage 1: Sector and system review
Purpose:
- Understand what existing hospitality systems do well.
- Identify allergen and workflow gaps.

Output:
- Initial requirement criteria list.

### Stage 2: Stakeholder interviews + brainstorming
Purpose:
- Capture role-specific needs and pain points.
- Agree operational priorities and compliance concerns.

Output:
- Draft functional and non-functional requirements.

### Stage 3: 1-to-1 role interviews + questionnaires + departmental focus groups
Purpose:
- Validate and refine requirements.
- Identify automation opportunities and workflow bottlenecks.

Output:
- Prioritised requirements baseline (Must/Should/Could).

---

## 3) Questionnaire design

## 3.1 Questionnaire format
- Type: Mixed (Likert + short free-text)
- Scale: 1 = Strongly Disagree, 5 = Strongly Agree
- Audience: kitchen, FOH, management
- Completion time: 8 to 10 minutes

## 3.2 Core questionnaire (all staff)
1. Our current order workflow is easy to follow during peak service.
2. Order mistakes occur because information is handwritten or repeated verbally.
3. Table status is always clear to all staff.
4. Special requests (including allergens) are communicated reliably to kitchen.
5. Re-keying information between systems causes delays.
6. Taking payment at table would improve speed and customer experience.
7. Real-time updates (orders, voids, changes) would reduce confusion.
8. A digital floor plan would help allocate and monitor tables better.
9. I would find allergen prompts at order-taking useful.
10. I would trust a digital allergen acknowledgment record for service support.
11. Training effort for a new system would be manageable.
12. I believe a single integrated system would improve service quality.

Free-text prompts:
- What is the most common delay in your shift?
- What causes the most customer complaints?
- What is one task that should be automated first?
- What allergen-related risk worries you most?

## 3.3 Management add-on questions
- Reporting on table turnover is currently difficult.
- We can easily audit allergen communication if required.
- Existing tools provide enough visibility across kitchen and FOH.
- Data retention/deletion is currently handled consistently.

---

## 4) 1-to-1 interview guides (semi-structured)

Each interview: 30 to 45 minutes.

## 4.1 Managing Director (Shane Breslin)
1. What are the top 3 operational pain points today?
2. Which KPIs matter most (turnaround, covers, complaints, average spend)?
3. What compliance concerns do you have around allergen handling?
4. Where do current systems/manual steps break down?
5. What budget and training constraints must the solution respect?
6. What outcomes would define project success in 3 months?

## 4.2 Head Chef (Damian McGettigan)
1. How are orders currently received and prioritised?
2. What information is often missing on tickets?
3. How are allergen alerts currently communicated?
4. Which ticket statuses should be visible to FOH in real time?
5. What would reduce rework and interrupted prep time?
6. How should special requests be displayed to kitchen?

## 4.3 Floor Managers (Lucy Coyle, Kyle Barr)
1. What are the biggest FOH bottlenecks during peak periods?
2. How do you track open tables and bill state currently?
3. Where do order amendments cause errors?
4. What payment flow would be fastest and least disruptive?
5. Which alerts should managers see first?
6. What should be mandatory before an order is sent?

## 4.4 Chef (Anthony Arnold)
1. What order details are essential for your station?
2. What makes kitchen tickets hard to act on quickly?
3. How should allergy flags be prioritised visually?
4. What causes delays in starter/main/desert status updates?
5. What 2 changes would most improve kitchen speed?

## 4.5 Hostess (Cara Farmer, rotational role)
1. What information do you need to seat guests quickly?
2. How often do table moves create confusion?
3. What floor plan features would help most?
4. How should wait-times and table readiness be displayed?
5. What handover info should follow the table when moved?

## 4.6 Floor Staff (Navinne Murray, Darragh Devlin)
1. What steps in taking an order are most time-consuming?
2. What order changes happen most often after initial entry?
3. When do allergen conversations usually happen?
4. What would help you avoid repeated trips to table/till/kitchen?
5. What mobile features would help you upsell appropriately?

---

## 5) Focus group design

## 5.1 Group sessions
- Kitchen group: Head Chef + Chef
- FOH group: Floor managers + hostess + floor staff
- Cross-functional group: management + kitchen + FOH

Session length:
- 45 to 60 minutes each

Format:
1. Current workflow mapping (as-is)
2. Pain point ranking
3. Future workflow sketch (to-be)
4. Prioritisation of features (Must/Should/Could)

## 5.2 Focus group prompts
- Where does service slow down most during peak?
- Which handoffs fail most often (FOH to kitchen / kitchen to FOH)?
- What allergen steps are currently weakest?
- Which 5 features should be delivered first?
- What should never be optional in the new system?
- What data should be visible by role only?

---

## 6) Simulated findings (for dissertation write-up)

Use these as realistic analysis outputs from the instruments above.

### 6.1 Common pain points identified
- Duplicate entry and handwritten notes cause avoidable errors.
- Table status is not always visible in real time to all staff.
- Order amendments are a frequent source of confusion.
- Allergen communication is inconsistent at busy times.
- FOH spend too much time walking between tables, till, and kitchen.

### 6.2 Stakeholder priority outcomes
- Faster order-to-kitchen time
- Fewer incorrect tickets
- Clear allergen prompts at ordering stage
- Better table turnover visibility
- At-table payment to reduce delays

### 6.3 Must-have requirements (derived)
1. Mobile order entry at table with instant kitchen/bar transmission.
2. Real-time table status and floor plan visibility.
3. Structured order status updates (starters/mains/deserts stages).
4. Allergen tagging per menu item with mandatory alert prompts.
5. Recorded allergen acknowledgment linked to order.
6. Bill updates in real time with split-bill support.
7. Role-based access to sensitive customer/allergen data.
8. Configurable data retention and deletion workflow (e.g., ~60 days where appropriate).

### 6.4 Should-have requirements
- Queue-busting mode for peak service.
- Item availability sync (off-menu alerts).
- Shift/role dashboards for managers.
- Basic review capture flow.

### 6.5 Could-have requirements
- Suggestive upsell prompts.
- Advanced performance analytics.
- Customer-facing allergen information view.

---

## 7) Requirements traceability starter table

| Requirement | Source instrument | Stakeholder(s) | Priority |
|---|---|---|---|
| Mobile table-side ordering | FOH questionnaire Q1/Q5 + FOH focus group | FOH, Managers | Must |
| Kitchen status updates | Kitchen interviews + cross-functional focus group | Kitchen, FOH | Must |
| Allergen prompt + acknowledgment | Core questionnaire Q9/Q10 + management interview | All groups | Must |
| Split bills and live totals | FOH interviews + questionnaire Q6 | FOH | Must |
| Role-based access control | Management interview + legal analysis | Management | Must |
| Data retention/deletion policy | Management add-on + legal analysis | Management | Must |

---

## 8) Ethics and reporting note (important)

If these were not run in real-world data collection, label them clearly as:
- simulated instruments,
- pilot-context requirement elicitation design,
- and/or scenario-based analysis artefacts.

This keeps your dissertation academically honest while still showing strong analysis planning and professional requirement engineering.
