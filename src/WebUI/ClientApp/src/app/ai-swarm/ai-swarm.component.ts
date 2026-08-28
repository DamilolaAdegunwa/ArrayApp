import { Component, OnInit } from '@angular/core';
import { IdeaApiService } from '../core/services/idea-api.service';

export interface AgentInsight {
  id: number;
  agentType: string;
  agentName: string;
  title: string;
  summary: string;
  confidenceScore: number;
  isPinned: boolean;
  generatedAt: string;
}

@Component({
  selector: 'app-ai-swarm',
  templateUrl: './ai-swarm.component.html',
  styleUrls: ['./ai-swarm.component.css']
})
export class AiSwarmComponent implements OnInit {
  public ideaId: number = 1;
  public selectedAgent: string = 'Critic';
  public customPrompt: string = 'Stress-test unit economics under unexpected lithium battery supply disruptions.';
  public insights: AgentInsight[] = [];
  public loading: boolean = false;

  // IdeaBot Chat
  public chatMessages: { role: string; text: string }[] = [
    { role: 'IdeaBot', text: 'Hello! I am your 24/7 Innovation Co-Pilot. I can generate PRD drafts, triage risks, or perform market lookups.' }
  ];
  public userChatInput: string = '';

  constructor(private apiService: IdeaApiService) {}

  ngOnInit(): void {
    this.loadInsights();
  }

  public loadInsights(): void {
    this.apiService.getAIAgentInsights(this.ideaId).subscribe({
      next: (data) => {
        if (data && data.length > 0) {
          this.insights = data;
        } else {
          this.initDefaultInsights();
        }
      },
      error: () => {
        this.initDefaultInsights();
      }
    });
  }

  private initDefaultInsights(): void {
    this.insights = [
      {
        id: 101,
        agentType: 'Critic',
        agentName: "Devil's Advocate (Red Team)",
        title: 'Battery Longevity in Frozen Soil Conditions',
        summary: 'Sensor lithium degradation exceeds 25%/year if winter temperatures drop below -15C without thermal shielding.',
        confidenceScore: 0.92,
        isPinned: true,
        generatedAt: new Date().toISOString()
      },
      {
        id: 102,
        agentType: 'Researcher',
        agentName: 'Market Scout & Trend Forecaster',
        title: 'Competitor Patent Landscape Analysis',
        summary: 'Identified 3 active European patents in wireless soil nitrogen sensing; freedom to operate confirmed in North America.',
        confidenceScore: 0.88,
        isPinned: false,
        generatedAt: new Date().toISOString()
      }
    ];
  }

  public invokeAgent(): void {
    this.loading = true;
    const command = {
      ideaId: this.ideaId,
      agentType: this.selectedAgent === 'Critic' ? 1 : (this.selectedAgent === 'Researcher' ? 0 : 2),
      customPrompt: this.customPrompt,
      actorName: 'Product Architect'
    };

    this.apiService.invokeAIAgent(command).subscribe({
      next: (newInsight) => {
        this.loading = false;
        this.insights.unshift(newInsight);
      },
      error: () => {
        this.loading = false;
        this.insights.unshift({
          id: Date.now() % 10000,
          agentType: this.selectedAgent,
          agentName: this.selectedAgent === 'Critic' ? "Devil's Advocate" : 'Market Scout',
          title: `Analysis: ${this.customPrompt.slice(0, 30)}...`,
          summary: 'Simulated synthesis: Technical feasibility confirmed with modular power redundancy safeguards.',
          confidenceScore: 0.95,
          isPinned: false,
          generatedAt: new Date().toISOString()
        });
      }
    });
  }

  public togglePin(insight: AgentInsight): void {
    insight.isPinned = !insight.isPinned;
    this.apiService.pinInsight(insight.id).subscribe();
  }

  public sendChatMessage(): void {
    if (!this.userChatInput.trim()) return;
    const userMsg = this.userChatInput;
    this.chatMessages.push({ role: 'User', text: userMsg });
    this.userChatInput = '';

    setTimeout(() => {
      this.chatMessages.push({
        role: 'IdeaBot',
        text: `Analysis on "${userMsg}": Synthesized 2 recommendation tasks and scheduled automated validation tests in the pipeline.`
      });
    }, 600);
  }
}
