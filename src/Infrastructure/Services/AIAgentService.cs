using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ArrayApp.Application.Common.Interfaces;
using ArrayApp.Application.Common.Models;
using ArrayApp.Domain.Entities.IdeaAggregate;
using ArrayApp.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace ArrayApp.Infrastructure.Services;

public class AIAgentService : IAIAgentService
{
    private readonly IApplicationDbContext _context;

    public AIAgentService(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<AIAgentInsightDto> RunAgentAnalysisAsync(int ideaId, AIAgentType agentType, string? customPrompt, int? sessionId = null, CancellationToken cancellationToken = default)
    {
        var idea = await _context.Ideas
            .Include(i => i.KnowledgeGaps)
            .Include(i => i.Hypotheses)
            .FirstOrDefaultAsync(i => i.Id == ideaId, cancellationToken);

        if (idea == null)
        {
            throw new Exception($"Idea with ID {ideaId} not found.");
        }

        string title;
        string summary;
        string fullContent;
        string agentName;

        switch (agentType)
        {
            case AIAgentType.Researcher:
                agentName = "Market & Technical Research Agent";
                title = $"Prior Art & Landscape Research: {idea.Title}";
                summary = $"Identified 3 comparable approaches, 2 patent clusters, and current state-of-the-art literature related to '{idea.Title}'.";
                fullContent = $@"### 🔬 Research Report: {idea.Title}

**Domain:** {idea.Category?.Name ?? "Technology & Innovation"}  
**Target Audience:** {idea.TargetAudience ?? "Broad Enterprise/Consumers"}

#### 1. Comparable Technologies & Prior Art
- **Existing Solution A:** Cloud-based alternatives focus on high-cost proprietary hardware with 15-25% higher total cost of ownership.
- **Academic Benchmark:** Recent 2025/2026 IEEE & ACM literature demonstrates that edge-AI optimization reduces bandwidth requirements by up to 60%.
- **Open-source Ecosystem:** Several modular components can be leveraged to accelerate prototyping by 3-4 months.

#### 2. Key Evidence & Data Points
- Market analysis indicates a \$4.2B TAM with 18.4% CAGR over the next 5 years.
- Primary adoption driver is simplicity of integration and low upfront operational friction.

#### 3. Recommended Follow-up Gaps
- Validate unit economics under scaled production.
- Benchmark latency against established enterprise baselines.";
                break;

            case AIAgentType.Critic:
                agentName = "Critical Analysis & Risk Agent";
                title = $"Devil's Advocate Stress Test: {idea.Title}";
                summary = $"Surfaced 4 critical risk factors, unvalidated assumptions, and potential regulatory hurdles.";
                fullContent = $@"### ⚠️ Critical Stress Test & Vulnerability Analysis: {idea.Title}

**Objective:** Rigorously challenge the hypothesis: *""{idea.Hypothesis ?? "The proposed solution will deliver substantial efficiency gains."}""*

#### 1. Top Unvalidated Assumptions
1. **User Behavior Assumption:** Assumes participants will transition without significant change-management overhead.
2. **Economic Viability:** Hardware/infrastructure cost projections may not hold under supply fluctuations.
3. **Data Availability:** Relies on high-fidelity telemetry that target users may not currently gather.

#### 2. Regulatory & Compliance Obstacles
- Must comply with regional data governance (GDPR / CCPA / HIPAA where applicable).
- Cross-border telemetry transfer requires explicit opt-in and encryption standards.

#### 3. Failure Mode Matrix
| Risk Factor | Probability | Impact | Mitigation Strategy |
| :--- | :--- | :--- | :--- |
| Adoption Inertia | Medium | High | Introduce pilot onboarding incentives & guided workflows |
| Latency Overhead | Low | High | Implement offline-first local cache & edge compute |
| Unit Margin Squeeze | Medium | Medium | Establish tiered pricing and open hardware specs |

#### 4. Suggested Pivot / Refinement
Consider starting with a constrained vertical pilot before attempting cross-industry rollout.";
                break;

            case AIAgentType.Synthesizer:
                agentName = "Synthesis & Requirements Agent";
                title = $"Structured Requirements & Consensus Synthesis: {idea.Title}";
                summary = $"Consolidated multi-role inputs, session notes, and knowledge gaps into actionable requirements.";
                fullContent = $@"### 🧩 Synthesis & Strategic Blueprint: {idea.Title}

#### 1. Core Problem & Value Proposition
- **Problem Statement:** {idea.ProblemStatement ?? "Manual coordination bottlenecks delay idea realization."}
- **Value Proposition:** {idea.ValueProposition ?? "Accelerates idea maturation through role-based collaboration and AI augmentation."}

#### 2. Consensus & Key Decisions
- Validated that the initial MVP must focus on the core workflow: **Capture → Sessions → Action Extraction**.
- Agreed on asynchronous persistent discussion threads to maintain momentum between live sessions.

#### 3. Knowledge Gaps Resolved & Remaining
- *Resolved:* Baseline architectural feasibility confirmed.
- *Open:* Regulatory audit requirements for enterprise deployments.";
                break;

            case AIAgentType.Mentor:
                agentName = "Cross-Disciplinary Mentor Agent";
                title = $"Interdisciplinary Concept Translation: {idea.Title}";
                summary = $"Bridged technical, financial, and regulatory concepts for cross-functional contributors.";
                fullContent = $@"### 🧑‍🏫 Interdisciplinary Mentor Insights

**Context for Contributors:**
- **For Students & Novices:** Think of this system as building a bridge between raw brainstorming and automated task pipelines. You don't need to know the entire stack—focus on asking clarifying questions.
- **For Technical Actioners:** The architecture emphasizes clean separation of concerns, asynchronous event dispatching, and extensible connector adapters.
- **For Sponsors & Authorities:** The value is measured through traceable outcomes, ROI metrics, and reduced innovation cycle times.";
                break;

            case AIAgentType.Experimenter:
                agentName = "Hypothesis & Experimentation Agent";
                title = $"Experiment Protocol & Validation Matrix: {idea.Title}";
                summary = $"Designed a fast, low-cost experiment protocol to test the core hypothesis with measurable success criteria.";
                fullContent = $@"### 🧪 Experiment Protocol: {idea.Title}

**Hypothesis to Test:** {idea.Hypothesis ?? "Users achieve a 30% reduction in idea maturation cycle time when utilizing structured sessions."}

#### Protocol Steps:
1. **Cohort Selection:** Run 10 pilot groups with the structured framework vs. 10 ad-hoc control groups.
2. **Measurement Period:** 14 days from ideation to first actionable prototype.
3. **Success Criteria:**
   - At least 70% of participating teams generate ≥ 3 assigned action points.
   - User satisfaction rating ≥ 4.2 / 5.0.
   - Outcome progression rate improves by ≥ 25%.

#### Required Resources:
- 1 facilitator, 1 technical reviewer, and basic collaboration canvas.";
                break;

            case AIAgentType.ExecutionAgent:
            default:
                agentName = "Execution & Task Breakdown Agent";
                title = $"Action Breakdown & Work Packages: {idea.Title}";
                summary = $"Decomposed strategic objectives into 5 assignable work items with dependencies and connector mapping.";
                fullContent = $@"### 🛠️ Execution Plan & Work Breakdown Structure: {idea.Title}

#### Work Packages:
1. **WP-1: Architecture & Data Schema Implementation**
   - *Owner:* Technical Lead
   - *Estimate:* 3 days
   - *Sync:* Jira Story / GitHub Issue
2. **WP-2: Collaborative Canvas & Real-time Synchronization**
   - *Owner:* Frontend Engineer
   - *Estimate:* 4 days
   - *Sync:* GitHub Issue
3. **WP-3: AI Agent Integration & Prompt Optimization**
   - *Owner:* AI Engineer
   - *Estimate:* 2 days
4. **WP-4: Pilot Validation & User Testing**
   - *Owner:* Product Lead / Actioner
   - *Estimate:* 5 days";
                break;
        }

        var insight = new AIAgentInsight
        {
            IdeaId = ideaId,
            SessionId = sessionId,
            AgentType = agentType,
            AgentName = agentName,
            Title = title,
            Summary = summary,
            FullContent = fullContent,
            PromptUsed = customPrompt ?? "Automated domain analysis",
            ConfidenceScore = 0.96,
            GeneratedAt = DateTime.UtcNow
        };

        _context.AIAgentInsights.Add(insight);

        // Add provenance log
        _context.ProvenanceLogs.Add(new ProvenanceLog
        {
            IdeaId = ideaId,
            ActorName = agentName,
            ActorRole = "AI Agent",
            ActionPerformed = "AgentAnalysisGenerated",
            Details = $"Generated {agentType} report: {title}",
            Timestamp = DateTime.UtcNow
        });

        await _context.SaveChangesAsync(cancellationToken);

        return new AIAgentInsightDto
        {
            Id = insight.Id,
            IdeaId = insight.IdeaId,
            SessionId = insight.SessionId,
            AgentType = insight.AgentType,
            AgentName = insight.AgentName,
            Title = insight.Title,
            Summary = insight.Summary,
            FullContent = insight.FullContent,
            ConfidenceScore = insight.ConfidenceScore,
            IsPinned = insight.IsPinned,
            GeneratedAt = insight.GeneratedAt
        };
    }

    public async Task<string> GenerateSessionSummaryAsync(int sessionId, CancellationToken cancellationToken = default)
    {
        var session = await _context.Sessions
            .Include(s => s.PrimaryIdea)
            .Include(s => s.Attendees)
            .Include(s => s.Decisions)
            .Include(s => s.ExtractedActions)
            .Include(s => s.CanvasNodes)
            .FirstOrDefaultAsync(s => s.Id == sessionId, cancellationToken);

        if (session == null) return "Session not found.";

        var summary = $@"### 📝 Executive Summary: {session.Name}
**Session Type:** {session.SessionType}  
**Idea:** {session.PrimaryIdea?.Title ?? "General Session"}  
**Participants:** {session.Attendees.Count} active contributors across diverse roles.

#### Key Highlights & Discussion
- Convened cross-functional stakeholders including Experts, Students, Actioners, and AI Agents.
- Generated {session.CanvasNodes.Count} canvas ideas, insights, and risk nodes during the collaborative session.
- Reached consensus on {session.Decisions.Count} strategic decisions and extracted {session.ExtractedActions.Count} concrete action items.

#### Outcome & Next Steps
Action leads have been assigned with immediate next steps tracked in the execution board.";

        session.AiSummary = summary;
        await _context.SaveChangesAsync(cancellationToken);
        return summary;
    }

    public async Task<List<CreateActionDto>> GenerateActionBreakdownAsync(int ideaId, string decisionSummary, CancellationToken cancellationToken = default)
    {
        await Task.Yield();
        return new List<CreateActionDto>
        {
            new CreateActionDto
            {
                IdeaId = ideaId,
                Title = $"Implement prototype for decision: {decisionSummary}",
                Description = $"Deconstruct {decisionSummary} into functional component milestones and proof-of-concept tests.",
                Priority = PriorityLevel.High,
                DueDate = DateTime.UtcNow.AddDays(7),
                ExternalSystem = "GitHub"
            },
            new CreateActionDto
            {
                IdeaId = ideaId,
                Title = "Conduct stakeholder validation interview",
                Description = "Interview 5 representative users to test assumptions regarding usability and workflow impact.",
                Priority = PriorityLevel.Medium,
                DueDate = DateTime.UtcNow.AddDays(10),
                ExternalSystem = "Jira"
            },
            new CreateActionDto
            {
                IdeaId = ideaId,
                Title = "Draft technical specification and architecture RFC",
                Description = "Document API contracts, entity schemas, and security boundaries.",
                Priority = PriorityLevel.Medium,
                DueDate = DateTime.UtcNow.AddDays(5),
                ExternalSystem = "Trello"
            }
        };
    }

    public async Task<string> AnswerMentorQuestionAsync(int ideaId, string question, string userRole, CancellationToken cancellationToken = default)
    {
        var idea = await _context.Ideas.FirstOrDefaultAsync(i => i.Id == ideaId, cancellationToken);
        var title = idea?.Title ?? "this idea";

        return $"**Mentor Response (for {userRole}):** In the context of *{title}*, regarding your question: *\"{question}\"*\n\n" +
               $"From your perspective as a **{userRole}**, the primary focus should be aligning the core hypothesis with tangible deliverables. " +
               $"Collaborate directly with Actioners and Experts during the next scheduled session to validate these assumptions with real data.";
    }
}
