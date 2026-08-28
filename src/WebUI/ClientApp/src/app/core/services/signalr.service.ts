import { Injectable } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { Subject, Observable } from 'rxjs';

export interface CanvasNodeUpdatePayload {
  ideaId: number;
  sessionId?: number;
  nodeId: number;
  nodeType: string;
  posX: number;
  posY: number;
  content: string;
  votesCount: number;
}

export interface PlaybookAdvancePayload {
  sessionId: number;
  playbookName: string;
  previousPhaseIndex: number;
  currentPhaseIndex: number;
  currentPhaseName: string;
  timeRemainingSeconds: number;
}

export interface AIAgentInsightPayload {
  ideaId: number;
  agentType: string;
  agentName: string;
  title: string;
  confidenceScore: number;
}

@Injectable({
  providedIn: 'root'
})
export class SignalRService {
  private hubConnection?: signalR.HubConnection;

  public canvasNodeUpdated$ = new Subject<CanvasNodeUpdatePayload>();
  public playbookAdvanced$ = new Subject<PlaybookAdvancePayload>();
  public aiAgentInsightGenerated$ = new Subject<AIAgentInsightPayload>();
  public speechActionsExtracted$ = new Subject<any>();
  public quadraticVoteCast$ = new Subject<any>();

  public isConnected = false;

  constructor() {
    this.initHubConnection();
  }

  public initHubConnection(): void {
    try {
      this.hubConnection = new signalR.HubConnectionBuilder()
        .withUrl('/hubs/ideas', {
          skipNegotiation: true,
          transport: signalR.HttpTransportType.WebSockets
        })
        .withAutomaticReconnect([0, 2000, 5000, 10000])
        .configureLogging(signalR.LogLevel.Warning)
        .build();

      this.hubConnection.on('CanvasNodeUpdated', (data: CanvasNodeUpdatePayload) => {
        this.canvasNodeUpdated$.next(data);
      });

      this.hubConnection.on('PlaybookPhaseAdvanced', (data: PlaybookAdvancePayload) => {
        this.playbookAdvanced$.next(data);
      });

      this.hubConnection.on('AIAgentInsightGenerated', (data: AIAgentInsightPayload) => {
        this.aiAgentInsightGenerated$.next(data);
      });

      this.hubConnection.on('SpeechActionsExtracted', (data: any) => {
        this.speechActionsExtracted$.next(data);
      });

      this.hubConnection.on('QuadraticVoteCast', (data: any) => {
        this.quadraticVoteCast$.next(data);
      });

      this.hubConnection.start()
        .then(() => {
          this.isConnected = true;
          console.log('[SignalRService] Connected to real-time innovation hub.');
        })
        .catch(err => {
          console.warn('[SignalRService] Live WebSocket not active yet; falling back to reactive polling mode.', err);
        });
    } catch (e) {
      console.warn('[SignalRService] Hub initialization skipped in non-browser context.');
    }
  }

  public joinIdeaRoom(ideaId: number): void {
    if (this.hubConnection && this.isConnected) {
      this.hubConnection.invoke('JoinIdeaRoom', ideaId).catch(console.error);
    }
  }

  public broadcastCanvasNode(payload: CanvasNodeUpdatePayload): void {
    if (this.hubConnection && this.isConnected) {
      this.hubConnection.invoke('BroadcastCanvasNode', payload).catch(console.error);
    }
  }
}
