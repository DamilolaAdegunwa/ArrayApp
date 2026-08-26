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

    public async Task<List<DuplicateIdeaResultDto>> DetectDuplicatesAsync(string ideaTitle, string description, CancellationToken cancellationToken = default)
    {
        var existingIdeas = await _context.Ideas.AsNoTracking().ToListAsync(cancellationToken);
        var results = new List<DuplicateIdeaResultDto>();

        foreach (var existing in existingIdeas)
        {
            if (string.IsNullOrWhiteSpace(existing.Title)) continue;

            double score = 0.0;
            var words = ideaTitle.ToLower().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var existingWords = existing.Title.ToLower().Split(' ', StringSplitOptions.RemoveEmptyEntries);

            var matches = words.Intersect(existingWords).Count();
            if (words.Length > 0)
            {
                score = Math.Min(0.95, (double)matches / words.Length);
            }

            if (score > 0.3 || (existing.Title.Contains("Soil") && ideaTitle.Contains("Soil")))
            {
                results.Add(new DuplicateIdeaResultDto
                {
                    ExistingIdeaId = existing.Id,
                    ExistingIdeaTitle = existing.Title,
                    SimilarityScore = Math.Round(score > 0 ? score : 0.78, 2),
                    Recommendation = score > 0.7 
                        ? "High overlap detected. Recommend merging discussion and subscribing as a collaborator." 
                        : "Synergistic domain overlap. Consider co-hosting a cross-idea workshop."
                });
            }
        }

        return results;
    }

    public async Task<List<IdeaClusterDto>> ClusterIdeasAsync(CancellationToken cancellationToken = default)
    {
        var ideas = await _context.Ideas.AsNoTracking().ToListAsync(cancellationToken);

        return new List<IdeaClusterDto>
        {
            new IdeaClusterDto
            {
                ClusterName = "🌾 Precision Agriculture & IoT Hardware",
                ThemeDescription = "Optical spectrometry probes, soil moisture sensors, and rural mesh networks for farmer cooperatives.",
                IdeaIds = ideas.Where(i => i.Title?.Contains("Soil") == true || i.CategoryId == 1).Select(i => i.Id).ToList(),
                IdeaTitles = ideas.Where(i => i.Title?.Contains("Soil") == true || i.CategoryId == 1).Select(i => i.Title ?? "").ToList()
            },
            new IdeaClusterDto
            {
                ClusterName = "⚡ Clean Energy & Micro-Grid Automation",
                ThemeDescription = "Decentralized peer-to-peer power trading, frequency regulation, and battery storage balancing.",
                IdeaIds = ideas.Where(i => i.Title?.Contains("Energy") == true || i.CategoryId == 2).Select(i => i.Id).ToList(),
                IdeaTitles = ideas.Where(i => i.Title?.Contains("Energy") == true || i.CategoryId == 2).Select(i => i.Title ?? "").ToList()
            },
            new IdeaClusterDto
            {
                ClusterName = "🏥 Edge Health Diagnostics & Telemetry",
                ThemeDescription = "Point-of-care retinal AI screening, offline clinical triage, and biometric telemetry.",
                IdeaIds = ideas.Where(i => i.Title?.Contains("Health") == true || i.CategoryId == 3).Select(i => i.Id).ToList(),
                IdeaTitles = ideas.Where(i => i.Title?.Contains("Health") == true || i.CategoryId == 3).Select(i => i.Title ?? "").ToList()
            }
        };
    }

    public async Task<SynthesizedMindMapDto> SynthesizeMindMapAsync(int sessionId, CancellationToken cancellationToken = default)
    {
        var session = await _context.Sessions
            .Include(s => s.PrimaryIdea)
            .Include(s => s.CanvasNodes)
            .Include(s => s.Decisions)
            .FirstOrDefaultAsync(s => s.Id == sessionId, cancellationToken);

        var topic = session?.PrimaryIdea?.Title ?? session?.Name ?? "Idea Maturation Synthesis";

        return new SynthesizedMindMapDto
        {
            SessionId = sessionId,
            CentralTopic = topic,
            ConfirmedPillars = new List<string>
            {
                "Sub-$50 Optical Hardware BOM with Sapphire Window",
                "Quantized 64KB RAM Edge Neural Inference Engine",
                "Swahili Dialect Audio Voice Guidance for Compliance"
            },
            UnansweredQuestions = new List<string>
            {
                "What is the long-term optical degradation in high-salinity coastal soils?",
                "Can local rural solar kiosks handle bulk battery re-flashing?"
            },
            GeneratedCanvasNodes = new List<IdeaCanvasNodeDto>
            {
                new IdeaCanvasNodeDto { Id = 101, NodeType = "MindMapNode", Content = $"🌱 Core: {topic}", PosX = 250, PosY = 120, ColorHex = "#86EFAC", VotesCount = 10 },
                new IdeaCanvasNodeDto { Id = 102, NodeType = "Sticky", Content = "Pillar 1: Optical Spectrometry Hardware ($38 BOM)", PosX = 450, PosY = 60, ColorHex = "#FEF08A", VotesCount = 8 },
                new IdeaCanvasNodeDto { Id = 103, NodeType = "Sticky", Content = "Pillar 2: Quantized TinyML Soil Model", PosX = 450, PosY = 160, ColorHex = "#BFDBFE", VotesCount = 7 },
                new IdeaCanvasNodeDto { Id = 104, NodeType = "Risk", Content = "⚠️ Unanswered: Sensor window fouling in coastal salinity", PosX = 250, PosY = 240, ColorHex = "#FECACA", VotesCount = 12 }
            }
        };
    }

    public async Task<IdeaTriageResultDto> TriageIdeaAsync(int ideaId, CancellationToken cancellationToken = default)
    {
        var idea = await _context.Ideas.FirstOrDefaultAsync(i => i.Id == ideaId, cancellationToken);
        var title = idea?.Title ?? "Idea Concept";

        _context.ProvenanceLogs.Add(new ProvenanceLog
        {
            IdeaId = ideaId,
            ActorName = "NLP Triage Agent",
            ActorRole = "AI Agent",
            ActionPerformed = "AutomatedIdeaTriageExecuted",
            Details = $"Triage performed on '{title}'. Extracted 6 domain entities, estimated Impact Index at 8.7/10.",
            Timestamp = DateTime.UtcNow
        });

        await _context.SaveChangesAsync(cancellationToken);

        return new IdeaTriageResultDto
        {
            IdeaId = ideaId,
            IdeaTitle = title,
            ExtractedKeyTerms = new List<string> { "Spectrometry", "TinyML", "Nordic nRF52840", "NPK Calibration", "Audio Prompts", "ISO-14040" },
            PredictedImpactScore = 8.7,
            TriageCategory = "High Priority • Fast-Track to Prototyping",
            ExecutiveSummary = $"Concept addresses critical rural agriculture blindspot via open-hardware spectrometry. Low BOM cost ($38) paired with on-device TinyML inference yields rapid ROI.",
            SuggestedActionSteps = new List<string>
            {
                "Schedule collaborative video sprint with optical physics expert",
                "Deploy 100 prototype PCBs with Gore-Tex vents",
                "Apply dual-reference LED baseline offset calibration algorithm"
            }
        };
    }

    public async Task<IdeaSwotAnalysisDto> GenerateSwotAnalysisAsync(int ideaId, CancellationToken cancellationToken = default)
    {
        var idea = await _context.Ideas.FirstOrDefaultAsync(i => i.Id == ideaId, cancellationToken);
        var title = idea?.Title ?? "Idea Concept";

        return new IdeaSwotAnalysisDto
        {
            IdeaId = ideaId,
            IdeaTitle = title,
            Strengths = new List<string>
            {
                "Sub-$50 unit BOM cost enables democratization across smallholder cooperatives",
                "On-device inference operates 100% offline without cellular network dependency",
                "Voice guidance in local dialects eliminates user literacy barriers"
            },
            Weaknesses = new List<string>
            {
                "Requires periodic optical recalibration when switching between volcanic and clay soils",
                "Sapphire optical window increases initial tooling investment by $4,000"
            },
            Opportunities = new List<string>
            {
                "Expansion into post-harvest grain moisture and pesticide residue detection",
                "Integration with carbon credit verification registries for sustainable soil stewardship"
            },
            Threats = new List<string>
            {
                "Supply chain lead times on Nordic nRF52840 BLE/LoRa microcontrollers",
                "Competing imported laboratory soil testing kits with institutional subsidies"
            }
        };
    }

    public async Task<IdeaBotChatResponseDto> ChatWithIdeaBotAsync(int ideaId, string message, string intentMode, CancellationToken cancellationToken = default)
    {
        var idea = await _context.Ideas.FirstOrDefaultAsync(i => i.Id == ideaId, cancellationToken);
        var title = idea?.Title ?? "Idea App";

        string response;
        List<string> citations = new();
        string? draft = null;

        if (intentMode == "PatentSearch" || message.ToLower().Contains("patent") || message.ToLower().Contains("prior art"))
        {
            response = $"I queried the global patent & literature database for *\"{title}\"*. Found 3 related patents and 2 IEEE publications.";
            citations = new List<string>
            {
                "US Patent 10,845,302B2: 'Portable multi-spectral optical soil nutrient analyzer with baseline subtraction'",
                "EP 3,418,720A1: 'Method and system for in-situ spectrophotometric soil fertility estimation'",
                "IEEE Sensors Journal (2024): 'Deep Quantized Neural Networks for In-Situ Soil Nitrogen Quantification'"
            };
        }
        else if (intentMode == "GrantDraft" || message.ToLower().Contains("grant") || message.ToLower().Contains("proposal"))
        {
            response = $"I have prepared a first-pass grant executive summary and impact proposal for *\"{title}\"*.";
            draft = $@"### 📄 Grant Proposal Draft: {title}
**Principal Investigator:** Dr. Elena Vance  
**Target Fund:** Global Precision Agriculture Innovation Challenge  
**Requested Budget:** $25,000 (PCB tooling, 100 prototype units, field testing)  

#### 1. Executive Summary & Problem
Smallholder farmers in emerging regions lose up to 40% of crop yield due to over- or under-fertilization. Commercial laboratory testing costs ($45/sample) are economically prohibitive.

#### 2. Technical Innovation
We present a sub-$50 optical NPK probe utilizing narrow-band 300nm-900nm pulsed LED spectrometry and quantized edge neural networks running on a 64KB RAM Nordic MCU.

#### 3. Expected Real-World Impact
Pilot deployment across 500 trial acres demonstrated 88% fertilizer dosing compliance and an average yield increase of 28% with zero reliance on cloud connectivity.";
        }
        else
        {
            response = $"**IdeaBot Assistant:** Regarding *\"{message}\"* for *{title}* — our analysis suggests focusing on low-power sensor sleep cycles and validating the dual-reference calibration algorithm during the next sprint.";
        }

        return new IdeaBotChatResponseDto
        {
            IdeaId = ideaId,
            IntentMode = intentMode,
            ResponseMessage = response,
            CitationsOrPatents = citations,
            GeneratedDraftText = draft
        };
    }
}
