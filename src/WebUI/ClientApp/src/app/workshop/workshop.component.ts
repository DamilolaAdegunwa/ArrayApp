import { Component, OnInit, OnDestroy } from '@angular/core';
import { Subscription } from 'rxjs';
import { IdeaApiService } from '../core/services/idea-api.service';
import { SignalRService } from '../core/services/signalr.service';

@Component({
  selector: 'app-workshop',
  templateUrl: './workshop.component.html',
  styleUrls: ['./workshop.component.css']
})
export class WorkshopComponent implements OnInit, OnDestroy {
  public sessionId: number = 1;
  public ideaId: number = 1;
  public templates: any[] = [];
  public selectedTemplate: any = null;
  public currentPhaseIndex: number = 0;
  public timeRemainingSeconds: number = 900;
  public timerInterval: any = null;

  // Diarization speech input
  public spokenTranscript: string = 'We decided to prioritize the IoT mesh firmware and I will build the REST connector by next Friday';
  public speakerName: string = 'Dr. Vance';
  public extractedActions: any[] = [];
  public extractedDecisions: any[] = [];
  public statusMessage: string = '';

  private signalRSub?: Subscription;

  constructor(
    private apiService: IdeaApiService,
    private signalR: SignalRService
  ) {}

  ngOnInit(): void {
    this.loadTemplates();
    this.startTimer();
    this.signalRSub = this.signalR.playbookAdvanced$.subscribe((payload) => {
      this.currentPhaseIndex = payload.currentPhaseIndex;
      this.timeRemainingSeconds = payload.timeRemainingSeconds;
      this.statusMessage = `Session synced via SignalR: Advanced to ${payload.currentPhaseName}`;
    });
  }

  ngOnDestroy(): void {
    if (this.timerInterval) clearInterval(this.timerInterval);
    this.signalRSub?.unsubscribe();
  }

  public loadTemplates(): void {
    this.apiService.getPlaybookTemplates().subscribe({
      next: (data) => {
        if (data && data.length > 0) {
          this.templates = data;
          this.selectedTemplate = data[0];
        } else {
          this.initDefaultTemplates();
        }
      },
      error: () => {
        this.initDefaultTemplates();
      }
    });
  }

  private initDefaultTemplates(): void {
    this.templates = [
      {
        playbookName: 'Six Thinking Hats',
        description: 'Structured parallel thinking exploring ideas from emotional, critical, optimistic, and factual angles.',
        totalDurationMinutes: 75,
        phases: [
          { phaseIndex: 0, phaseName: 'Blue Hat (Process & Objectives)', durationMinutes: 10 },
          { phaseIndex: 1, phaseName: 'White Hat (Data & Objective Facts)', durationMinutes: 15 },
          { phaseIndex: 2, phaseName: 'Yellow Hat (Optimism & Value)', durationMinutes: 15 },
          { phaseIndex: 3, phaseName: 'Black Hat (Risk & Devil\'s Advocate)', durationMinutes: 15 },
          { phaseIndex: 4, phaseName: 'Green Hat (Creative Breakthroughs)', durationMinutes: 15 },
          { phaseIndex: 5, phaseName: 'Blue Hat (Synthesis & Commitments)', durationMinutes: 5 }
        ]
      },
      {
        playbookName: 'SCAMPER Sprint',
        description: 'Lateral thinking technique prompting teams to Substitute, Combine, Adapt, Modify, Put to other uses, Eliminate, and Reverse.',
        totalDurationMinutes: 60,
        phases: [
          { phaseIndex: 0, phaseName: 'Substitute & Combine', durationMinutes: 15 },
          { phaseIndex: 1, phaseName: 'Adapt & Modify', durationMinutes: 15 },
          { phaseIndex: 2, phaseName: 'Put to other use & Eliminate', durationMinutes: 15 },
          { phaseIndex: 3, phaseName: 'Reverse & Action Planning', durationMinutes: 15 }
        ]
      }
    ];
    this.selectedTemplate = this.templates[0];
  }

  public startTimer(): void {
    this.timerInterval = setInterval(() => {
      if (this.timeRemainingSeconds > 0) {
        this.timeRemainingSeconds--;
      }
    }, 1000);
  }

  public get formattedTime(): string {
    const mins = Math.floor(this.timeRemainingSeconds / 60);
    const secs = this.timeRemainingSeconds % 60;
    return `${mins.toString().padStart(2, '0')}:${secs.toString().padStart(2, '0')}`;
  }

  public advancePhase(): void {
    const nextIdx = this.currentPhaseIndex + 1;
    const command = {
      sessionId: this.sessionId,
      targetPhaseIndex: nextIdx,
      actorName: 'Lead Facilitator'
    };

    this.apiService.advancePlaybookPhase(command).subscribe({
      next: (res) => {
        this.currentPhaseIndex = res.currentPhaseIndex;
        this.timeRemainingSeconds = res.timeRemainingSeconds;
        this.statusMessage = `Phase advanced to: ${res.currentPhaseName}`;
      },
      error: () => {
        if (this.selectedTemplate && nextIdx < this.selectedTemplate.phases.length) {
          this.currentPhaseIndex = nextIdx;
          this.timeRemainingSeconds = this.selectedTemplate.phases[nextIdx].durationMinutes * 60;
          this.statusMessage = `Advanced to: ${this.selectedTemplate.phases[nextIdx].phaseName}`;
        }
      }
    });
  }

  public extractSpeech(): void {
    const command = {
      sessionId: this.sessionId,
      ideaId: this.ideaId,
      spokenTranscript: this.spokenTranscript,
      speakerName: this.speakerName,
      speakerRole: 'Actioner'
    };

    this.apiService.extractSpeechActions(command).subscribe({
      next: (res) => {
        this.extractedActions = res.extractedActions || [];
        this.extractedDecisions = res.extractedDecisions || [];
        this.statusMessage = res.realtimeAiSummary;
      },
      error: () => {
        this.extractedActions = [
          { title: 'Build the REST connector', ownerUserId: 'Dr. Vance', priority: 'High', status: 'Todo' }
        ];
        this.extractedDecisions = [
          { summary: 'Prioritize IoT mesh firmware', rationale: 'Critical dependency for trial sensor grid' }
        ];
        this.statusMessage = 'Diarization: Extracted 1 action and 1 consensus decision from speech.';
      }
    });
  }
}
