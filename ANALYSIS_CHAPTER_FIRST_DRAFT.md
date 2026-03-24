# Chapter 3: Analysis (First Draft)

## 3.1 Introduction
This chapter explains how the proposed solution was identified and justified for The Banks Restaurant, Strabane. The analysis is presented as a structured simulation, using pilot-context roles and requirement instruments from the project plan.

The chapter follows the three-stage method defined in the proposal: sector/system review, stakeholder interviews, and role-level validation through questionnaire and focus groups. Outputs are translated into functional and non-functional requirements, then used to justify market position, technology choice, business case, risk management, ethics, and delivery planning.

A governance decision applies across the chapter: Shane Breslin is the only managerial stakeholder and the highest security role. No floor-manager role is included.

### 3.1.1 Scope boundaries used in this chapter
The analysis is scoped to operational workflow and allergen safety in one system. In scope: table state, order flow, menu/menu-item management, ingredient/allergen linkage, and allergen consent at ordering. Out of scope: hotel management, staff rotas, stock management, and till/payment integration (external API availability is uncertain).

---

## 3.2 Stage 1: Sector and System Review
### 3.2.1 Aim and approach
Stage 1 examined how small hospitality venues in Ireland and Northern Ireland currently manage tables, ordering, kitchen handoff, and allergen communication. The focus was practical: what works in normal service and what fails during peak periods.

Review criteria were speed of order transmission, visibility of service state, amendment handling, allergen-record consistency, usability on shared devices, role-based access, and affordability.

### 3.2.2 Key findings from sector review
Three recurring patterns emerged: hybrid paper/digital practice leads to duplicate entry; FOH-to-kitchen exceptions rely heavily on verbal handoff; and table/order state is not consistently shared in real time. For allergens, the key weakness is inconsistency between what is discussed with the customer and what is recorded for kitchen execution.

### 3.2.3 Options explored and narrowed
Several solution options were considered:
- **Option A: Keep existing process with light policy improvements** (checklists, training refresh, clearer paper forms).
- **Option B: Separate specialist tools** (one for tables, one for orders, one for compliance records).
- **Option C: Single integrated RMS with allergen workflow embedded from order capture to kitchen display and audit trail.**

Option A was low-cost but unlikely to solve root causes. Option B improved capability but increased re-keying and fragmented accountability. Option C required higher setup effort but best matched operational consistency, traceability, and role-based control.

### 3.2.4 Ideas investigated then dropped
A number of ideas were deprioritised: customer self-order as primary flow, voice-command kitchen interactions, and phase-one expansion to hotel/rota/stock modules. Open manager dashboards for multiple manager users were also dropped due to the single-manager governance model.

Stage 1 output was a criteria list used directly in Stage 2 and Stage 3 instruments.

### 3.2.5 Existing market solutions and gap analysis
The market review considered common small-venue stacks: standalone POS, table tools, kitchen displays, and separate allergen references. The main gap was integration quality. In the pilot context, allergen handling is often a free-text add-on, role permissions are inconsistent across tools, and operational context is not always linked to ingredient/allergen data at ordering time.

The project therefore targets a focused web-based RMS rather than a broad enterprise POS replacement: small-venue fit, clearer governance, and embedded allergen consent in normal order flow.

### 3.2.6 Choice of platform and technology
The selected platform is a web application, allowing access on mixed devices without native app distribution. This suits the pilot’s constraints: shared hardware, short training windows, and incremental delivery.

The implementation direction is ASP.NET Core MVC with C# and a relational model. It supports role-based access, auditable operations, and maintainable business logic for table/order/menu/allergen workflows. It also aligns with the existing project environment and enables clear separation between UI, control flow, and data handling.

The stack supports key non-functional goals: security, testability, and supportability. It also keeps future integration options open while avoiding dependency on uncertain till APIs in this dissertation scope.

---

## 3.3 Stage 2: Stakeholder Interviews and Brainstorming
### 3.3.1 Stakeholders and governance assumption
The analysis used three stakeholder groups: management (Shane only), kitchen (Head Chef and Chef), and FOH (hostess and floor staff). Governance is centralised to one manager account for policy settings, role assignment, retention controls, and audit access.

### 3.3.2 Interview themes
Semi-structured interview design focused on pain points, KPIs, compliance expectations, ticket quality, FOH-kitchen handoff, amendment handling, and constraints around training, cost, and adoption.

### 3.3.3 Cross-role findings
Three findings were repeated: (1) throughput delays are mostly communication delays, especially state-change synchronisation; (2) exceptions such as amendments and allergen alerts are where workflows break down most; (3) confidence in service speed is higher than confidence in allergen-record consistency.

### 3.3.4 Stage 2 output
Stage 2 produced a draft requirement set: mobile ordering, menu/menu-item control, real-time status, ingredient-aware allergen prompts, role-based visibility, peak-time reliability, and clear policy accountability.

---

## 3.4 Stage 3: Role Interviews, Questionnaire, and Focus Groups
### 3.4.1 Instrument design
Stage 3 validated requirements using a 12-question Likert questionnaire with free text, management governance questions, role-level interview prompts, and kitchen/FOH/cross-functional focus groups. The goal was prioritised, role-traceable requirements.

### 3.4.2 Questionnaire interpretation (simulated)
Questionnaire items tested peak workflow quality, communication reliability, table-state visibility, real-time updates, and confidence in allergen prompts/acknowledgment.

Simulated interpretation indicates agreement that:
- re-keying and handwritten repetition contribute to delays/errors;
- real-time updates are expected to reduce confusion;
- allergen prompts and acknowledgment records are considered useful and trustworthy if embedded in normal flow.

Free-text responses confirmed that delays and complaints cluster around handoff failures and that allergen risk rises when communication is interrupted.

### 3.4.3 Focus group synthesis
Focus groups mapped “as-is” versus “to-be” workflows and ranked features using Must/Should/Could. Kitchen prioritised actionable tickets and clear allergy flags. FOH prioritised mobile ordering, table-state clarity, and faster amendments. Cross-functional discussion prioritised single-source records, legal defensibility, and role-specific visibility.

### 3.4.4 Prioritised requirements baseline
The final baseline derived from Stage 3 is:

**Must-have (core delivery):**
1. Mobile order entry at table with instant transmission to kitchen/bar.
2. Real-time table status and floor plan visibility.
3. Structured order status updates (starters/mains/desserts).
4. Allergen tagging per menu item with mandatory prompts at order-taking.
5. Recorded allergen acknowledgment linked to each relevant order.
6. Ingredient-level linkage to menu items for allergen visibility and safer substitutions.
7. Role-based access to customer and allergen-sensitive information.
8. Configurable data retention/deletion workflow (target baseline: approximately 60 days where appropriate).

**Should-have (phase two candidate):**
- queue-busting mode;
- item-availability sync;
- management dashboard views.

**Could-have (future enhancement):**
- advanced performance analytics;
- customer-facing allergen information view.

### 3.4.5 Functional and non-functional requirements expression
For implementation planning, requirements are represented as user stories and non-functional constraints.

**Example functional stories:**
- As FOH staff, I want to submit an order from the table so that kitchen receives it immediately without re-entry.
- As kitchen staff, I want allergy flags highlighted in a standard format so that safe preparation is clear and fast.
- As management (Shane), I want access to audit logs for allergen acknowledgments so that compliance evidence is retrievable.
- As FOH staff, I want menu items linked to ingredients/allergens so that substitutions can be handled more safely.

**Example non-functional requirements:**
- The system should provide near real-time update propagation for order and table state changes.
- The interface should be learnable with short onboarding appropriate to mixed-experience staff.
- Access control should enforce least privilege by role, with top-level policy/security permissions restricted to the managerial account.
- Audit records should be retained and deleted according to configurable policy.

### 3.4.6 Traceability and requirement confidence
A core strength is traceability: requirements map to questionnaire items, interviews, and focus-group ranking. This reduces feature-led drift and supports defensible prioritisation, particularly for allergen controls.

---

## 3.5 Business Case (Approx. 250 words)
The business case for the proposed RMS is based on service efficiency, error reduction, and risk control. In a small venue, relatively small operational delays can compound during peak sessions, reducing table turnover and customer satisfaction. By replacing fragmented handwritten/verbal workflows with a single integrated process, the venue is likely to reduce wasted staff movement and repeated order clarification.

Expected benefits include faster order-to-kitchen time, fewer ticket errors, better menu/ingredient consistency, and stronger visibility of live service state. For management, centralised reporting and compliance records improve decision quality and reduce administrative friction.

The allergen component strengthens the case further. Mandatory prompts and acknowledgment records help shift allergy communication from informal memory-based practice to structured and auditable workflow. This can reduce the likelihood of serious incidents and supports legal defensibility if complaints arise.

Costs are likely to include software implementation effort, device provisioning, onboarding/training time, and temporary disruption during transition. There may also be recurring costs for maintenance and support. However, these costs are justified where they replace hidden operational losses from rework, delays, and avoidable customer dissatisfaction.

Overall, the analysis indicates a positive value proposition: moderate implementation cost in exchange for measurable gains in throughput, quality, and compliance confidence.

---

## 3.6 Project Risks and Risk Management (Approx. 250 words)
The project carries technical, operational, and people-related risks.

A major technical risk is system reliability during peak service. If updates lag or devices fail, staff may revert to ad hoc paper processes, creating inconsistency. Mitigation includes offline-tolerant workflows where feasible, clear fallback procedures, and staged pilot rollout before full dependency.

A key operational risk is process mismatch: if the digital workflow does not reflect real shift behaviour, staff may bypass required steps (especially under time pressure). Mitigation includes role-based co-design, scenario testing of amendments/voids/allergy cases, and enforcing mandatory fields only where operationally essential.

Data/compliance risk is significant for allergen records and customer-linked information. Inadequate access controls or unclear retention policy could create legal exposure. Mitigation includes strict role-based access, management-only policy configuration (Shane at top security level), audit logs, and explicit retention/deletion schedules.

Adoption risk is also important. Staff may resist change if perceived as slower than current habits. Mitigation includes short practical training, role-specific guides, super-user support during early shifts, and iterative improvement using staff feedback.

Scope risk should be controlled by delivering Must-have features first and deferring Should/Could features. This protects timeline and quality, ensuring that safety-critical and throughput-critical capabilities are stabilised before enhancement work.

---

## 3.7 Ethical Considerations (Approx. 250 words)
This project raises both research ethics and system ethics considerations.

First, this chapter uses simulated instruments and simulated findings based on realistic pilot roles and workflows. Ethical academic practice requires this to be explicit. The analysis therefore labels outputs as scenario-based artefacts for planning and requirement engineering, not as claims of completed real-world empirical data collection.

Second, food allergen handling has direct safety implications. Ethically, system design should prioritise harm prevention over convenience features. Mandatory allergen prompts, visible warnings, and acknowledgment capture are justified because they reduce the chance of omission at critical moments.

Third, the system processes staff actions and potentially customer-linked records. Ethical handling requires data minimisation, role-restricted access, and retention limits. Not all users should see all data. The security model in this draft places highest-level permissions with the single managerial stakeholder (Shane), while other users receive least-privilege access aligned to role duties.

Fourth, staff-facing ethics include fairness and usability. A system that is difficult to use under pressure may increase stress or encourage unsafe workarounds. Therefore, usability and training are ethical as well as technical concerns.

Finally, transparency and accountability are essential. Where allergen acknowledgments are captured, records should be accurate, time-stamped, and retrievable for legitimate review. This supports trust for customers, staff, and management while discouraging informal undocumented practices.

---

## 3.8 Project Plan for Deliverables (Approx. 250 words)
The delivery plan follows incremental implementation aligned to requirement priority.

**Phase 1: Foundation and core workflow design**
- confirm detailed use cases for table order lifecycle and kitchen receipt;
- define role model and permission matrix (management, kitchen, FOH);
- finalise menu-item, ingredient, and allergen data model, prompt logic, and acknowledgment record format.

**Phase 2: Must-have implementation**
- implement mobile table-side ordering and real-time order transmission;
- implement table/floor status model and shared visibility;
- implement structured course-status updates;
- implement menu/menu-item and ingredient linkage for allergen-aware ordering;
- implement allergen tagging, prompts, and mandatory acknowledgment path;
- implement role-filtered views for FOH, kitchen, and management.

**Phase 3: Security, compliance, and validation**
- enforce role-based access control with top-level management-only security controls;
- implement retention/deletion configuration and audit logging;
- run scenario-based validation for peak service, amendments, and allergen edge cases.

**Phase 4: Pilot readiness and refinement**
- complete role-specific training materials;
- run limited pilot sessions and capture feedback;
- stabilise high-impact issues and performance bottlenecks;
- freeze baseline for dissertation implementation/evaluation chapters.

Progress will be tracked against acceptance criteria tied to Must-have requirements. Should-have features are only admitted when Must-have flows are stable. This protects safety-critical objectives and ensures deliverables remain aligned with dissertation scope and timeline.

---

## 3.9 Conclusion
The analysis identifies a clear problem pattern: service delays and risk exposure are concentrated at communication handoffs, exception handling, and allergen disclosure points. Through a three-stage requirement elicitation method, the project moves from broad sector understanding to role-specific, traceable requirements grounded in realistic restaurant operations.

The resulting solution direction is an integrated RMS that embeds allergen safety directly into operational flow rather than treating compliance as a separate afterthought. Centralising top-level governance in one managerial stakeholder (Shane Breslin) also clarifies accountability for security, policy, and audit access.

This chapter therefore establishes a justified, prioritised requirement baseline for implementation: deliver core mobile ordering, real-time visibility, menu/ingredient structure, and auditable allergen workflow first; then expand to secondary optimisation features in later phases. The next chapter can now translate these requirements into concrete system design decisions.
