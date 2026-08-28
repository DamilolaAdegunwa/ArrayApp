# Phase 2 Implementation Plan: Full Angular Enterprise Frontend, SignalR Mesh & End-to-End Delivery

## Overview
Phase 1 achieved 100% completion of the backend CQRS architecture across all 12 Epics and 4 Milestones defined in [UserStory.md](file:///Users/dammy/Documents/GitHub/ArrayApp/UserStory.md), verified with 39 passing unit tests and 0 build errors/warnings.

**Phase 2** elevates this backend foundation into a comprehensive, production-ready enterprise solution by building out the full-scale Angular client architecture in [src/WebUI/ClientApp/](file:///Users/dammy/Documents/GitHub/ArrayApp/src/WebUI/ClientApp/), integrating real-time SignalR state synchronization, adding comprehensive integration tests, and validating production release packaging.

---

## User Review Required

> [!IMPORTANT]
> The Angular client app is located in `src/WebUI/ClientApp/` (Angular 14+ / TypeScript). We will augment this Angular application with modern standalone components, SignalR streaming services, and responsive Bootstrap 5 / SVG visual design, matching the native Angular architecture requested by the user.

---

## Workstreams Breakdown

### Workstream 1: Full-Featured Angular ClientApp Architecture (`src/WebUI/ClientApp`)

Grouped into modular, high-performance Angular feature components:

1. **Idea Studio & 10-Dimensional Maturation Component (`src/WebUI/ClientApp/src/app/idea-studio/`)**:
   - 10-Dimensional input wizard with dynamic sliders for ICE/RICE scoring.
   - Stage progression indicator visualizing all 10 maturity stages (Raw $\rightarrow$ Evolving).
   - Lineage tree visualizer showing parent ideas, forks, and merged lineages.
2. **Spatial 2D Infinite Canvas Component (`src/WebUI/ClientApp/src/app/canvas/`)**:
   - Interactive SVG/HTML5 canvas with pan, zoom, drag-and-drop sticky notes, mind map nodes, and action cards.
   - Live quadratic voting widget (Cost = $\text{Votes}^2$) deducting credits with real-time feedback.
   - One-click auto-clustering organizing scattered thoughts into structured columns.
3. **Multi-Format Workshop Playbook Component (`src/WebUI/ClientApp/src/app/workshop/`)**:
   - Guided playbook facilitator UI (SCAMPER, Six Thinking Hats, Rapid Hackathons).
   - Synchronized phase timer with auto-advancing stages.
4. **Autonomous AI Swarm Console (`src/WebUI/ClientApp/src/app/ai-swarm/`)**:
   - Real-time insight cards from Devil's Advocate, Market Scout, Feasibility Auditor, and Compliance Sentinel.
   - Pinned insight toggle and 24/7 IdeaBot co-pilot chat drawer.
5. **Executive ROI & Risk Portfolio Dashboard (`src/WebUI/ClientApp/src/app/executive/`)**:
   - 10-stage funnel drop-off visualization with conversion percentages.
   - 4-quadrant scatter matrix (Quick Wins, Strategic Bets, Low Hanging Fruit, Complex Initiatives).
   - Net financial impact summary (Total Cost Savings, Revenue, ROI %).
6. **Governance & Connectors Hub (`src/WebUI/ClientApp/src/app/governance/`)**:
   - W3C DID Ed25519 Verifiable Credential viewer with cryptographic tamper check.
   - Jira / GitHub / Linear bi-directional connector configuration panel.

---

### Workstream 2: SignalR Real-Time Mesh Integration

1. **SignalR Service (`src/WebUI/ClientApp/src/app/core/signalr.service.ts`)**:
   - Manages WebSocket connection to `/hubs/ideas` and `/hubs/sessions`.
   - Dispatches live updates for canvas node movements, votes, playbook phase advances, and diarized action items directly to Angular RxJS Observables.

---

### Workstream 3: End-to-End Integration Testing Suite

1. **Integration Tests (`tests/Application.IntegrationTests/Ideas/`)**:
   - `IdeaLifecycleIntegrationTests.cs`: Full cycle test from idea creation, 10-D updating, forking, and merging.
   - `AIAgentSwarmIntegrationTests.cs`: Invocation of Critic and Market Scout agents with insight generation and pinning.
   - `EnterpriseConnectorsIntegrationTests.cs`: End-to-end action dispatching and inbound webhook reconciliation.
   - `ProvenanceChainIntegrationTests.cs`: End-to-end hash chain calculation and W3C DID certificate issuance.

---

### Workstream 4: Production Build & Packaging Verification

1. **Angular Client Build**:
   - Run `npm run build` inside `src/WebUI/ClientApp` to verify TypeScript typings and clean AOT compilation.
2. **Release Build & Verification**:
   - Run `dotnet build -c Release` and `dotnet publish` to guarantee production deployment readiness.

---

## Verification Plan

### Automated Tests
- `dotnet test tests/Application.UnitTests/Application.UnitTests.csproj` (all 34+ unit tests passing).
- `dotnet test tests/Domain.UnitTests/Domain.UnitTests.csproj` (all 5+ domain tests passing).
- `dotnet test tests/Application.IntegrationTests/Application.IntegrationTests.csproj` (end-to-end integration tests).
- `cd src/WebUI/ClientApp && npm run build` (clean Angular production build).

### Manual Verification
- Launch application via `dotnet run --project src/WebUI/ArrayApp.WebUI.csproj`.
- Verify navigation across Idea Studio, 2D Canvas, AI Swarm, Playbook Facilitator, Executive ROI Dashboard, and Governance Hub.
