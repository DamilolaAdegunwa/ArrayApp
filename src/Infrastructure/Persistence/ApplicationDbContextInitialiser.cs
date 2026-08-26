using ArrayApp.Domain.Entities;
using ArrayApp.Domain.Entities.CategoryAggregate;
using ArrayApp.Domain.Entities.IdeaAggregate;
using ArrayApp.Domain.Entities.SessionAggregate;
using ArrayApp.Domain.Enums;
using ArrayApp.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ArrayApp.Infrastructure.Persistence;

public class ApplicationDbContextInitialiser
{
    private readonly ILogger<ApplicationDbContextInitialiser> _logger;
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;

    public ApplicationDbContextInitialiser(ILogger<ApplicationDbContextInitialiser> logger, ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
    {
        _logger = logger;
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task InitialiseAsync()
    {
        try
        {
            if (_context.Database.IsSqlServer())
            {
                await _context.Database.MigrateAsync();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while initialising the database.");
            throw;
        }
    }

    public async Task SeedAsync()
    {
        try
        {
            await TrySeedAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while seeding the database.");
            throw;
        }
    }

    public async Task TrySeedAsync()
    {
        try
        {
            // Default roles
            //var administratorRole = new ApplicationRole("Administrator");
            //var adm = await _roleManager.FindByNameAsync("Administrator");
            //var adm = await _context.ApplicationRoles.FirstOrDefaultAsync(r => r.Name != null && r.Name.ToLower() == "Administrator".ToLower()); //_roleManager.FindByNameAsync("Administrator");
            var adm = await _context.Roles.FirstOrDefaultAsync(r => true);
            if (adm != null) { return; }

            var administratorRole = new ApplicationRole { Name = "Administrator" };
            //administratorRole.Name = "admin";
            if (_roleManager.Roles.All(r => r.Name != administratorRole.Name))
            {
                await _roleManager.CreateAsync(administratorRole);
            }

            // Default users
            var administrator = new ApplicationUser { UserName = "administrator@localhost", Email = "administrator@localhost" };

            if (_userManager.Users.All(u => u.UserName != administrator.UserName))
            {
                await _userManager.CreateAsync(administrator, "Administrator1!");
                await _userManager.AddToRolesAsync(administrator, new[] { administratorRole.Name });
            }

            // Seed Categories
            if (!_context.Categories.Any())
            {
                var agritech = new Category { Name = "Agritech & IoT", Description = "Smart agriculture, soil sensing, and food security" };
                var cleanEnergy = new Category { Name = "Clean Energy & Smart Grid", Description = "Decentralized energy, microgrids, and renewables" };
                var health = new Category { Name = "Digital Health & Bio", Description = "Telehealth, diagnostic AI, and patient care" };
                var sustainability = new Category { Name = "Circular Economy", Description = "Zero-waste, recycling, and sustainable materials" };
                var edtech = new Category { Name = "EdTech & Learning", Description = "Gamified learning and knowledge networks" };

                _context.Categories.AddRange(agritech, cleanEnergy, health, sustainability, edtech);
                await _context.SaveChangesAsync();
            }

            // Seed Rich Idea Products
            if (!_context.Ideas.Any())
            {
                var agritechCat = await _context.Categories.FirstAsync(c => c.Name.Contains("Agritech"));
                var energyCat = await _context.Categories.FirstAsync(c => c.Name.Contains("Energy"));
                var healthCat = await _context.Categories.FirstAsync(c => c.Name.Contains("Health"));
                var circularCat = await _context.Categories.FirstAsync(c => c.Name.Contains("Circular"));

                #region Idea 1: AI Agricultural Soil Monitor
                var idea1 = new Idea
                {
                    Title = "AI-Powered Low-Cost Soil & Crop Health Monitor",
                    Tagline = "Affordable optical edge-AI sensor array delivering instant fertilizer & irrigation guidance.",
                    Description = "A $40 solar-powered IoT sensor probe that tests soil nitrogen, phosphorus, potassium, and moisture in real time, delivering smartphone recommendations to rural smallholders.",
                    ProblemStatement = "Over 500M smallholder farmers lack access to soil testing laboratories, leading to either fertilizer overuse (destroying soil microbiology) or severe under-fertilization (cutting yields by 40%). Laboratory tests cost $80+ per sample and take weeks.",
                    Opportunity = "Sub-$50 optical spectrophotometry coupled with tinyML microcontrollers can bring laboratory-grade nutrient estimation directly into the farmer's hands.",
                    Hypothesis = "Deploying an edge-AI multispectral soil probe with voice-guided SMS/App alerts will reduce fertilizer expenditure by 25% and increase seasonal crop yield by 18%.",
                    TargetAudience = "Smallholder farmer cooperatives, agronomists, regional agricultural ministries, NGOs.",
                    ValueProposition = "Instant soil diagnostics in 30 seconds at 1/20th the cost of commercial lab alternatives.",
                    Constraints = "Dust, extreme tropical moisture, limited rural 4G connectivity, solar battery longevity.",
                    Unknowns = "Spectral absorption curve variance across clay vs. sandy soils; sensor degradation over 24-month field exposure.",
                    Evidence = "2025 field tests across 50 trial plots in Kenya showed 94% correlation with standard laboratory mass spectrometry.",
                    DesiredOutcome = "Open-hardware manufacturing blueprint, mobile companion app, and 10,000 unit cooperative pilot.",
                    MaturityStage = IdeaMaturityStage.Building,
                    Visibility = IdeaVisibility.Public,
                    CategoryId = agritechCat.Id,
                    CreatorUserId = administrator.Id,
                    Rating = 4.9,
                    Upvotes = 34,
                    FollowersCount = 28,
                    ViewsCount = 312
                };

                // Knowledge gaps
                idea1.KnowledgeGaps.Add(new KnowledgeGap
                {
                    Title = "Wavelength Calibration Across Volcanic Soil Types",
                    Description = "Evaluate whether near-infrared LEDs require recalibration when operating in high-iron volcanic soils.",
                    DomainArea = "Spectral Physics",
                    Priority = PriorityLevel.High,
                    Status = KnowledgeGapStatus.Resolved,
                    ResolutionDetails = "Firmware updated with dual-reference white-point LED calibration algorithm.",
                    ResolvedAt = DateTime.UtcNow.AddDays(-5)
                });
                idea1.KnowledgeGaps.Add(new KnowledgeGap
                {
                    Title = "LoRaWAN vs. Cellular Telemetry in Deep Valleys",
                    Description = "Assess packet drop rate when base stations are shadowed by hilly terrain.",
                    DomainArea = "Telecommunications",
                    Priority = PriorityLevel.Medium,
                    Status = KnowledgeGapStatus.Open
                });

                // Hypotheses & Experiments
                var hyp1 = new IdeaHypothesis
                {
                    Statement = "Farmers will trust and follow automated fertilizer dosage recommendations provided in local audio dialects.",
                    Rationale = "Audio prompts overcome literacy barriers and build high user trust."
                };
                hyp1.Experiments.Add(new IdeaExperiment
                {
                    Title = "Dialect Audio Prompt Pilot with 40 Cooperative Members",
                    Description = "Compare compliance rates between text SMS alerts vs. automated voice notes in Swahili.",
                    Protocol = "Send 20 members text alerts, 20 members voice notes across a 30-day fertilizer application window.",
                    RequiredResources = "Twilio voice gateway account, 40 test handsets.",
                    ExpectedMetric = "≥ 80% application adherence in voice cohort vs. ≤ 50% in SMS cohort.",
                    ActualResult = "Voice cohort achieved 88% adherence; SMS cohort achieved 47%.",
                    Learnings = "Audio voice notes significantly improved confidence in dosage guidelines.",
                    Status = ExperimentStatus.Validated,
                    StartedAt = DateTime.UtcNow.AddDays(-20),
                    CompletedAt = DateTime.UtcNow.AddDays(-3)
                });
                idea1.Hypotheses.Add(hyp1);

                // Decisions & Actions
                var dec1 = new IdeaDecision
                {
                    Summary = "Adopt Nordic nRF52840 MCU with BLE Mesh + LoRaWAN hybrid module",
                    Rationale = "Lowest power draw during deep sleep (1.8uA), enabling 18-month battery life with a small 2W solar cell.",
                    Context = "Session #1 Architecture Review with Professional Engineers.",
                    DecidedAt = DateTime.UtcNow.AddDays(-12)
                };
                idea1.Decisions.Add(dec1);

                idea1.Actions.Add(new IdeaAction
                {
                    Title = "Design and 3D print IP67 waterproof probe enclosure",
                    Description = "Enclosure must protect optical lenses from dirt while allowing easy soil insertion.",
                    Priority = PriorityLevel.High,
                    Status = ActionItemStatus.Done,
                    DueDate = DateTime.UtcNow.AddDays(-2),
                    CompletedAt = DateTime.UtcNow.AddDays(-1),
                    ExternalSystem = "GitHub",
                    ExternalReferenceKey = "issue#12",
                    ExternalUrl = "https://github.com/arrayapp/soil-probe/issues/12"
                });
                idea1.Actions.Add(new IdeaAction
                {
                    Title = "Compile quantized edge neural network for nutrient inference",
                    Description = "Convert TensorFlow Lite model into C++ array runnable on 64KB RAM MCU.",
                    Priority = PriorityLevel.High,
                    Status = ActionItemStatus.InProgress,
                    DueDate = DateTime.UtcNow.AddDays(4),
                    ExternalSystem = "Jira",
                    ExternalReferenceKey = "IDEA-104",
                    ExternalUrl = "https://jira.atlassian.net/browse/IDEA-104"
                });
                idea1.Actions.Add(new IdeaAction
                {
                    Title = "Conduct regulatory approval meeting with Agriculture Ministry",
                    Description = "Present calibration data to safety and calibration board.",
                    Priority = PriorityLevel.Medium,
                    Status = ActionItemStatus.Todo,
                    DueDate = DateTime.UtcNow.AddDays(12),
                    ExternalSystem = "Slack",
                    ExternalReferenceKey = "slack-triage-482"
                });

                // Canvas nodes
                idea1.CanvasNodes.Add(new IdeaCanvasNode { NodeType = "MindMapNode", Content = "🌱 Core Optical Probe", PosX = 300, PosY = 200, ColorHex = "#86EFAC", VotesCount = 14 });
                idea1.CanvasNodes.Add(new IdeaCanvasNode { NodeType = "Sticky", Content = "Need UV-resistant resin for sensor window", PosX = 480, PosY = 160, ColorHex = "#FEF08A", VotesCount = 8 });
                idea1.CanvasNodes.Add(new IdeaCanvasNode { NodeType = "Risk", Content = "⚠️ Moisture condensation inside optical chamber", PosX = 480, PosY = 260, ColorHex = "#FECACA", VotesCount = 11 });
                idea1.CanvasNodes.Add(new IdeaCanvasNode { NodeType = "Action", Content = "✅ Test Gore-Tex acoustic membrane for venting", PosX = 660, PosY = 260, ColorHex = "#BFDBFE", VotesCount = 9 });

                // Discussion Channels & AI Reports
                var genChan = new DiscussionChannel { Name = "general", Description = "General discussion on soil probe project", IsDefault = true };
                genChan.Messages.Add(new DiscussionMessage { SenderName = "Elena Vance (Professional)", SenderRole = "Professional", Content = "Uploaded the optical calibration curves from lab run #4. NPK detection r² is 0.94." });
                genChan.Messages.Add(new DiscussionMessage { SenderName = "Marcus Thorne (Sponsor)", SenderRole = "Sponsor", Content = "Great progress! Our innovation fund is ready to sponsor 100 prototype units for field trials." });
                genChan.Messages.Add(new DiscussionMessage { SenderName = "AI Research Agent", SenderRole = "AI Agent", Content = "Market alert: A comparable patent expired last month, freeing up optical chamber lens geometries for open-source use.", IsAiGenerated = true, AiAgentType = "Researcher" });
                idea1.DiscussionChannels.Add(genChan);

                // AI Insight
                idea1.AIAgentInsights.Add(new AIAgentInsight
                {
                    AgentType = AIAgentType.Critic,
                    AgentName = "Critical Analysis & Risk Agent",
                    Title = "Comprehensive Stress Test: Field Durability & Adoption",
                    Summary = "Identified sensor fouling and moisture ingress as primary physical vulnerabilities; recommended automated optical self-cleaning sequence.",
                    FullContent = "### Critical Analysis\n1. **Optical Fouling:** Clay soils coat the sapphire glass window. *Mitigation:* Integrate a mini ultrasonic vibrating transducer.\n2. **Adoption Friction:** Farmers require trust validation. *Mitigation:* Pilot alongside established agronomists.",
                    ConfidenceScore = 0.98,
                    IsPinned = true
                });

                // Outcome
                idea1.Outcomes.Add(new IdeaOutcome
                {
                    Title = "Phase 1 Functional Soil Telemetry Prototype",
                    Summary = "Completed 20 fully working hardware prototypes tested across 500 acres of cooperative maize fields.",
                    Type = OutcomeType.Prototype,
                    EstimatedCostSavings = 45000,
                    RevenueGenerated = 12000,
                    ImpactedUsersCount = 650,
                    EstimatedRoiPercent = 210,
                    RetrospectiveNotes = "Achieved sub-$40 BOM cost and successfully transmitted real-time NPK readings over 6km distance.",
                    KeyLearnings = "Direct voice messages in local dialects increased compliance by over 80%."
                });
                #endregion

                #region Idea 2: Decentralized Clean Energy Microgrid
                var idea2 = new Idea
                {
                    Title = "Decentralized Community Clean Energy Microgrid",
                    Tagline = "Autonomous peer-to-peer solar energy trading using smart bi-directional inverters.",
                    Description = "Enables residential solar owners to trade surplus clean energy with neighboring homes and local clinics using automated micro-tariffs, reducing grid transmission losses by 60%.",
                    ProblemStatement = "Excess solar energy generated during midday peak hours is curtailed or sold back to the centralized grid at unfair wholesale rates, while neighboring low-income households suffer from high energy poverty.",
                    Opportunity = "Local neighborhood microgrids allow peer-to-peer automated load balancing and equitable energy pricing.",
                    Hypothesis = "Localized peer-to-peer energy dispatching will lower consumer electricity bills by 22% while increasing solar asset ROI for prosumers by 35%.",
                    TargetAudience = "Suburban housing estates, remote rural townships, municipal utility providers.",
                    ValueProposition = "Zero-loss local green energy exchange with automated smart-meter settlement.",
                    MaturityStage = IdeaMaturityStage.Validating,
                    Visibility = IdeaVisibility.Public,
                    CategoryId = energyCat.Id,
                    CreatorUserId = administrator.Id,
                    Rating = 4.8,
                    Upvotes = 27,
                    FollowersCount = 19,
                    ViewsCount = 184
                };
                idea2.DiscussionChannels.Add(new DiscussionChannel { Name = "general", Description = "Microgrid project discussion", IsDefault = true });
                idea2.Actions.Add(new IdeaAction
                {
                    Title = "Draft open protocol specification for inverter communication",
                    Description = "Define JSON-RPC over MQTT schema for telemetry and bidding.",
                    Priority = PriorityLevel.High,
                    Status = ActionItemStatus.InProgress,
                    DueDate = DateTime.UtcNow.AddDays(6),
                    ExternalSystem = "GitHub",
                    ExternalReferenceKey = "issue#5"
                });
                #endregion

                #region Idea 3: Accessible Telehealth Triage
                var idea3 = new Idea
                {
                    Title = "Accessible Multilingual AI Emergency Triage Protocol",
                    Tagline = "Voice-first AI triage assistant for rapid acute symptom categorization in resource-limited clinics.",
                    Description = "A certified open clinical protocol running on edge tablets that conducts 2-minute voice triage in 14 native languages, routing urgent cases to doctors and advising home care for mild ailments.",
                    ProblemStatement = "Rural emergency clinics experience 4-6 hour waiting times, resulting in delayed care for critical emergencies.",
                    Opportunity = "Standardized clinical algorithm assisted by voice-AI can safely pre-triage patients and flag high-risk red flags.",
                    Hypothesis = "Clinical voice triage will reduce emergency wait times for critical patients by 40% with zero missed red-flag escalations.",
                    TargetAudience = "District hospitals, rural clinics, paramedic services.",
                    MaturityStage = IdeaMaturityStage.Planned,
                    Visibility = IdeaVisibility.Public,
                    CategoryId = healthCat.Id,
                    CreatorUserId = administrator.Id,
                    Rating = 4.7,
                    Upvotes = 19,
                    FollowersCount = 15,
                    ViewsCount = 140
                };
                idea3.DiscussionChannels.Add(new DiscussionChannel { Name = "general", Description = "Telehealth discussion", IsDefault = true });
                #endregion

                #region Idea 4: Urban Food Waste to Biogas
                var idea4 = new Idea
                {
                    Title = "Autonomous Urban Food Waste to Clean Biogas Micro-Digester",
                    Tagline = "Smart, odor-free community anaerobic digester transforming restaurant food waste into cooking fuel.",
                    Description = "Modular containerized anaerobic digester with automated pH/temperature optimization and bio-filter odor control for restaurant clusters.",
                    ProblemStatement = "Urban commercial food waste produces millions of tons of methane in landfills while urban eateries pay high prices for LPG cooking gas.",
                    MaturityStage = IdeaMaturityStage.Exploring,
                    Visibility = IdeaVisibility.Public,
                    CategoryId = circularCat.Id,
                    CreatorUserId = administrator.Id,
                    Rating = 4.5,
                    Upvotes = 12,
                    FollowersCount = 9,
                    ViewsCount = 95
                };
                idea4.DiscussionChannels.Add(new DiscussionChannel { Name = "general", Description = "Biogas project discussion", IsDefault = true });
                #endregion

                _context.Ideas.AddRange(idea1, idea2, idea3, idea4);
                await _context.SaveChangesAsync();

                // Seed Live Sessions
                var session1 = new Session
                {
                    Name = "AI Soil Probe Hardware & Firmware Sprint #3",
                    Description = "Live cross-functional workshop to review optical calibration data, PCB layout, and assign field trial responsibilities.",
                    SessionType = SessionType.Workshop,
                    SessionStatus = SessionStatus.Live,
                    ScheduledStartTime = DateTime.UtcNow.AddMinutes(-30),
                    ActualStartTime = DateTime.UtcNow.AddMinutes(-30),
                    Duration = TimeSpan.FromHours(1.5),
                    MeetingUrl = "https://meet.arrayapp.io/session-soil-probe-sprint3",
                    PrimaryIdeaId = idea1.Id,
                    AgendaNotes = "1. Review NIR LED response curves\n2. PCB manufacturing quotes\n3. Assign LoRaWAN antenna testing\n4. Extract next action items"
                };

                session1.Attendees.Add(new SessionParticipant { UserId = administrator.Id.ToString(), DisplayName = "Dr. Elena Vance (Host)", Role = ParticipantRole.Professional, IsHost = true });
                session1.Attendees.Add(new SessionParticipant { UserId = "user-marcus", DisplayName = "Marcus Thorne (Sponsor)", Role = ParticipantRole.Sponsor });
                session1.Attendees.Add(new SessionParticipant { UserId = "user-sarah", DisplayName = "Sarah Chen (Actioner)", Role = ParticipantRole.Actioner });
                session1.Attendees.Add(new SessionParticipant { UserId = "ai-critic", DisplayName = "Critical Analysis Bot (AI)", Role = ParticipantRole.Researcher, IsAiAgent = true, AiAgentType = "Critic" });

                session1.Polls.Add(new SessionPoll
                {
                    Question = "Should we include USB-C charging alongside the 2W solar panel?",
                    Options = new List<SessionPollOption>
                    {
                        new SessionPollOption { OptionText = "Yes, essential for indoor bench testing", VotesCount = 12 },
                        new SessionPollOption { OptionText = "No, adds $1.20 BOM cost and sealing risk", VotesCount = 3 }
                    }
                });

                _context.Sessions.Add(session1);
                await _context.SaveChangesAsync();

                // Seed User Reputations & Leaderboard
                var rep1 = new UserReputation
                {
                    UserId = administrator.Id.ToString(),
                    TotalPoints = 1420,
                    IdeasHelpedCount = 14,
                    ActionsCompletedCount = 18,
                    OutcomesAchievedCount = 3,
                    KnowledgeGapsResolvedCount = 8,
                    SessionsFacilitatedCount = 6,
                    PrimaryReputationTitle = "Master Idea Builder"
                };
                rep1.Badges.Add(new UserBadge { BadgeType = BadgeType.IdeaBuilder, Title = "Master Idea Builder", Description = "Brought 3 ideas to tangible real-world outcomes.", Icon = "🏆" });
                rep1.Badges.Add(new UserBadge { BadgeType = BadgeType.ActionLeader, Title = "Action Leader", Description = "Completed over 15 high-impact execution tasks.", Icon = "🛠️" });
                rep1.Badges.Add(new UserBadge { BadgeType = BadgeType.IdeaCatalyst, Title = "Idea Catalyst", Description = "Active collaborator across multiple diverse ideas.", Icon = "⚡" });

                var rep2 = new UserReputation
                {
                    UserId = "user-marcus",
                    TotalPoints = 980,
                    IdeasHelpedCount = 9,
                    ActionsCompletedCount = 12,
                    OutcomesAchievedCount = 2,
                    KnowledgeGapsResolvedCount = 4,
                    PrimaryReputationTitle = "Idea Sponsor"
                };
                rep2.Badges.Add(new UserBadge { BadgeType = BadgeType.IdeaSponsor, Title = "Idea Sponsor", Description = "Provided funding and commercial backing for innovations.", Icon = "💰" });
                rep2.Badges.Add(new UserBadge { BadgeType = BadgeType.Connector, Title = "Master Connector", Description = "Connected teams with essential industry partners.", Icon = "🤝" });

                var rep3 = new UserReputation
                {
                    UserId = "user-sarah",
                    TotalPoints = 760,
                    IdeasHelpedCount = 8,
                    ActionsCompletedCount = 11,
                    OutcomesAchievedCount = 1,
                    KnowledgeGapsResolvedCount = 5,
                    PrimaryReputationTitle = "Knowledge Contributor"
                };
                rep3.Badges.Add(new UserBadge { BadgeType = BadgeType.KnowledgeContributor, Title = "Knowledge Contributor", Description = "Resolved 5 critical domain knowledge gaps.", Icon = "📚" });

                _context.UserReputations.AddRange(rep1, rep2, rep3);
                await _context.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during database seeding.");
        }
        
    }
}
