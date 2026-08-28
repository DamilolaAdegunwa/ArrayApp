import { Component, OnInit, OnDestroy } from '@angular/core';
import { Subscription } from 'rxjs';
import { IdeaApiService } from '../core/services/idea-api.service';
import { SignalRService } from '../core/services/signalr.service';

export interface CanvasNode {
  id: number;
  ideaId: number;
  nodeType: string;
  content: string;
  posX: number;
  posY: number;
  colorHex: string;
  votesCount: number;
  authorName: string;
}

@Component({
  selector: 'app-canvas',
  templateUrl: './canvas.component.html',
  styleUrls: ['./canvas.component.css']
})
export class CanvasComponent implements OnInit, OnDestroy {
  public ideaId: number = 1;
  public nodes: CanvasNode[] = [];
  public userCredits: number = 75;
  public statusMessage: string = '';
  private signalRSub?: Subscription;

  constructor(
    private apiService: IdeaApiService,
    private signalR: SignalRService
  ) {}

  ngOnInit(): void {
    this.loadNodes();
    this.signalRSub = this.signalR.canvasNodeUpdated$.subscribe((updated) => {
      const existing = this.nodes.find(n => n.id === updated.nodeId);
      if (existing) {
        existing.posX = updated.posX;
        existing.posY = updated.posY;
        existing.content = updated.content;
        existing.votesCount = updated.votesCount;
      } else {
        this.nodes.push(updated as any);
      }
    });
  }

  ngOnDestroy(): void {
    this.signalRSub?.unsubscribe();
  }

  public loadNodes(): void {
    this.apiService.getCanvasNodes(this.ideaId).subscribe({
      next: (nodes) => {
        if (nodes && nodes.length > 0) {
          this.nodes = nodes;
        } else {
          this.initDefaultNodes();
        }
      },
      error: () => {
        this.initDefaultNodes();
      }
    });
  }

  private initDefaultNodes(): void {
    this.nodes = [
      { id: 1, ideaId: 1, nodeType: 'Problem', content: 'Unpredictable aquifer depletion in western plains', posX: 60, posY: 80, colorHex: '#FDE68A', votesCount: 12, authorName: 'Agronomist' },
      { id: 2, ideaId: 1, nodeType: 'Hypothesis', content: 'Sub-surface LoRaWAN sensor mesh triggers micro-dosing valves', posX: 360, posY: 80, colorHex: '#A7F3D0', votesCount: 19, authorName: 'IoT Architect' },
      { id: 3, ideaId: 1, nodeType: 'Decision', content: 'Adopt open SoilData protocol rather than proprietary lock-in', posX: 660, posY: 80, colorHex: '#BAE6FD', votesCount: 28, authorName: 'CTO' },
      { id: 4, ideaId: 1, nodeType: 'ActionCard', content: 'Deploy 50 battery test rigs in Nebraska sector 4', posX: 60, posY: 280, colorHex: '#DDD6FE', votesCount: 8, authorName: 'Field Engineer' },
      { id: 5, ideaId: 1, nodeType: 'Sticky', content: 'What is the RF range through wet topsoil after flash rains?', posX: 360, posY: 280, colorHex: '#FED7AA', votesCount: 15, authorName: 'Student' }
    ];
  }

  public addSticky(): void {
    const newNode: CanvasNode = {
      id: Date.now() % 100000,
      ideaId: this.ideaId,
      nodeType: 'Sticky',
      content: 'New idea note...',
      posX: 200 + (Math.random() * 200),
      posY: 180 + (Math.random() * 150),
      colorHex: '#FEF08A',
      votesCount: 0,
      authorName: 'Innovator'
    };

    this.nodes.push(newNode);
    this.apiService.saveCanvasNode(newNode).subscribe();
    this.signalR.broadcastCanvasNode({ ...newNode, nodeId: newNode.id });
  }

  public castQuadraticVote(node: CanvasNode): void {
    // Formula: 1 vote = 1 credit, next vote costs quadratic multiplier
    const voteCost = 4; // Example cost for +2 votes
    if (this.userCredits >= voteCost) {
      this.userCredits -= voteCost;
      node.votesCount += 2;
      this.statusMessage = `Cast 2 Quadratic Votes on "${node.content.slice(0, 20)}..." (Deducted ${voteCost} credits. Remaining: ${this.userCredits})`;
      this.apiService.voteCanvasNode(this.ideaId, node.id, 2).subscribe();
    } else {
      this.statusMessage = 'Insufficient voting credits! Earn more by reviewing ideas or advancing playbooks.';
    }
  }

  public autoCluster(): void {
    this.apiService.autoClusterCanvas(this.ideaId).subscribe({
      next: (clustered) => {
        if (clustered && clustered.length > 0) {
          this.nodes = clustered;
        } else {
          this.realignGrid();
        }
        this.statusMessage = 'Auto-clustered canvas nodes into structured thematic columns.';
      },
      error: () => {
        this.realignGrid();
        this.statusMessage = 'Realigned canvas nodes into structured grid layout.';
      }
    });
  }

  private realignGrid(): void {
    let col = 0;
    let row = 0;
    this.nodes.forEach(n => {
      n.posX = 60 + (col * 300);
      n.posY = 80 + (row * 190);
      col++;
      if (col >= 3) {
        col = 0;
        row++;
      }
    });
  }
}
