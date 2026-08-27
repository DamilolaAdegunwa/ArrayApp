#pragma warning disable
#pragma info disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ArrayApp.Application.Common.Interfaces;

namespace ArrayApp.Infrastructure.Services;

// =========================================================================================================
// [NEW CORE ARCHITECTURAL ADDITION]: SessionPlaybookService
// Provides pre-configured workshop playbooks with step-by-step facilitation agendas, prompts & role guides
// =========================================================================================================
public class SessionPlaybookService : ISessionPlaybookService
{
    private static readonly List<WorkshopPlaybookDto> Playbooks = new()
    {
        new WorkshopPlaybookDto
        {
            FormatId = "brainstorm",
            Name = "SCAMPER & Crazy 8s Divergent Brainstorm",
            Description = "Fast-paced divergent ideation technique designed to push creative boundaries and generate seed ideas.",
            RecommendedDurationMinutes = 60,
            RecommendedRoles = new List<string> { "Student", "Creator", "Professional", "Audience" },
            Phases = new List<PlaybookPhaseDto>
            {
                new PlaybookPhaseDto
                {
                    PhaseNumber = 1,
                    Title = "Problem Framing & Empathy Map",
                    DurationMinutes = 10,
                    Goal = "Align on core user pain points and opportunity spaces.",
                    FacilitatorInstructions = "Review the 10-dimension problem statement and invite Student participants to ask clarifying questions.",
                    SuggestedPrompts = new List<string> { "What is the hidden assumption here?", "How does a smallholder farmer experience this pain point today?" }
                },
                new PlaybookPhaseDto
                {
                    PhaseNumber = 2,
                    Title = "SCAMPER Matrix Ideation",
                    DurationMinutes = 25,
                    Goal = "Apply Substitute, Combine, Adapt, Modify, Put to other uses, Eliminate, Reverse triggers.",
                    FacilitatorInstructions = "Instruct participants to drop colored sticky notes on the 2D canvas corresponding to SCAMPER letters.",
                    SuggestedPrompts = new List<string> { "What happens if we eliminate the LCD display and use Bluetooth audio instead?", "Can we combine solar harvesting into the probe casing?" }
                },
                new PlaybookPhaseDto
                {
                    PhaseNumber = 3,
                    Title = "Crazy 8s Sketching & 1-Click AI Seed Synthesis",
                    DurationMinutes = 15,
                    Goal = "Produce rapid visual concepts and leverage AI agents to synthesize seed ideas.",
                    FacilitatorInstructions = "Trigger the AI Seed Idea Generator to surface unexpected cross-domain analogies.",
                    SuggestedPrompts = new List<string> { "Sketch the simplest possible field calibration workflow in 60 seconds." }
                },
                new PlaybookPhaseDto
                {
                    PhaseNumber = 4,
                    Title = "Dot Voting & Action Extraction",
                    DurationMinutes = 10,
                    Goal = "Converge on top 3 ideas and dispatch tasks directly to Jira/GitHub.",
                    FacilitatorInstructions = "Allow each attendee 3 votes on canvas nodes, then convert winning stickies to assignable Action tickets.",
                    SuggestedPrompts = new List<string> { "Which concept has the highest ICE impact-to-effort ratio?" }
                }
            }
        },
        new WorkshopPlaybookDto
        {
            FormatId = "review",
            Name = "Six Thinking Hats Deep-Dive Review",
            Description = "Structured lateral thinking method evaluating the concept from neutral facts, emotion, risks, benefits, and creativity.",
            RecommendedDurationMinutes = 75,
            RecommendedRoles = new List<string> { "Professional", "Authority", "Researcher", "Experimenter" },
            Phases = new List<PlaybookPhaseDto>
            {
                new PlaybookPhaseDto
                {
                    PhaseNumber = 1,
                    Title = "White Hat: Pure Facts & Datasets",
                    DurationMinutes = 15,
                    Goal = "Audit empirical evidence and spectroscopic calibration data without subjective bias.",
                    FacilitatorInstructions = "Examine the R² = 0.941 correlation dataset against lab standards.",
                    SuggestedPrompts = new List<string> { "What data is missing regarding coastal saline soils?" }
                },
                new PlaybookPhaseDto
                {
                    PhaseNumber = 2,
                    Title = "Black Hat: Critical Risks & Failure Modes",
                    DurationMinutes = 20,
                    Goal = "Surface regulatory, thermal drift, and mechanical window fouling vulnerabilities.",
                    FacilitatorInstructions = "Encourage Authority and Critic AI agents to stress-test failure conditions.",
                    SuggestedPrompts = new List<string> { "Under what soil conditions will this optical probe fail completely?" }
                },
                new PlaybookPhaseDto
                {
                    PhaseNumber = 3,
                    Title = "Yellow Hat: Value & Long-Term Upside",
                    DurationMinutes = 20,
                    Goal = "Map out the economic value proposition and total addressable savings ($420k+).",
                    FacilitatorInstructions = "Engage Sponsor and Connector roles to evaluate scaling pathways.",
                    SuggestedPrompts = new List<string> { "How can cooperatives monetize the aggregate soil telemetry data?" }
                },
                new PlaybookPhaseDto
                {
                    PhaseNumber = 4,
                    Title = "Blue Hat: Synthesis & Decision Sign-Off",
                    DurationMinutes = 20,
                    Goal = "Issue formal regulatory and architecture sign-offs.",
                    FacilitatorInstructions = "Record final consensus and update idea maturity stage from In Review to Planned.",
                    SuggestedPrompts = new List<string> { "Are all critical unknowns accounted for in the risk register?" }
                }
            }
        },
        new WorkshopPlaybookDto
        {
            FormatId = "pitch",
            Name = "Investor & Sponsor Milestone Pitch",
            Description = "Executive valuation and grant milestone pitching agenda for securing capital commitments.",
            RecommendedDurationMinutes = 45,
            RecommendedRoles = new List<string> { "Sponsor", "Connector", "Authority", "Actioner" },
            Phases = new List<PlaybookPhaseDto>
            {
                new PlaybookPhaseDto
                {
                    PhaseNumber = 1,
                    Title = "The 10-Dimension Executive Narrative",
                    DurationMinutes = 15,
                    Goal = "Deliver structured problem, hypothesis, and $14.2B market opportunity presentation.",
                    FacilitatorInstructions = "Present the 10-D Idea Product Card and verifiable ROI metrics (340% ROI).",
                    SuggestedPrompts = new List<string> { "Why this team, why now, and what is the unfair technological advantage?" }
                },
                new PlaybookPhaseDto
                {
                    PhaseNumber = 2,
                    Title = "Q&A and Financial Due Diligence",
                    DurationMinutes = 20,
                    Goal = "Address unit economics, $48 BOM constraints, and supply chain lead times.",
                    FacilitatorInstructions = "Allow Sponsor and Authority participants to probe risk mitigations.",
                    SuggestedPrompts = new List<string> { "What is the breakeven timeline across the 500 trial acres?" }
                },
                new PlaybookPhaseDto
                {
                    PhaseNumber = 3,
                    Title = "Grant Pledge & Milestone Commitment",
                    DurationMinutes = 10,
                    Goal = "Record live sponsorship pledges and cryptographic realization certificate triggers.",
                    FacilitatorInstructions = "Trigger Sponsor action dispatch for the $25,000 pilot grant milestone.",
                    SuggestedPrompts = new List<string> { "Confirm release milestones for the first 100 prototype units." }
                }
            }
        }
    };

    public async Task<WorkshopPlaybookDto> GetPlaybookTemplateAsync(string formatType, CancellationToken cancellationToken = default)
    {
        var playbook = Playbooks.FirstOrDefault(p => string.Equals(p.FormatId, formatType, StringComparison.OrdinalIgnoreCase))
                       ?? Playbooks[0];

        return await Task.FromResult(playbook);
    }

    public async Task<List<WorkshopPlaybookDto>> GetAllPlaybooksAsync(CancellationToken cancellationToken = default)
    {
        return await Task.FromResult(Playbooks);
    }
}
