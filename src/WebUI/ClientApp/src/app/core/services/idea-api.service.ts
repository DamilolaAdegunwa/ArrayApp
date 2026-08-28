import { Injectable, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class IdeaApiService {
  private baseUrl: string;

  constructor(private http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    this.baseUrl = baseUrl;
  }

  // 1. Idea Products & 10 Dimensions
  public getIdeaDimensions(ideaId: number): Observable<any> {
    return this.http.get(`${this.baseUrl}api/ideaproducts/${ideaId}/dimensions`);
  }

  public updateIdeaDimensions(command: any): Observable<any> {
    return this.http.put(`${this.baseUrl}api/ideaproducts/${command.ideaId}/dimensions`, command);
  }

  public forkIdea(ideaId: number, command: any): Observable<any> {
    return this.http.post(`${this.baseUrl}api/ideaproducts/${ideaId}/fork`, command);
  }

  public mergeIdeas(command: any): Observable<any> {
    return this.http.post(`${this.baseUrl}api/ideaproducts/merge`, command);
  }

  public getIdeaLineage(ideaId: number): Observable<any> {
    return this.http.get(`${this.baseUrl}api/ideaproducts/${ideaId}/lineage`);
  }

  // 2. 10-Role Capacity Matrix
  public executeRoleAction(command: any): Observable<any> {
    return this.http.post(`${this.baseUrl}api/rolecapacity/execute`, command);
  }

  public getRoleHistory(ideaId: number): Observable<any> {
    return this.http.get(`${this.baseUrl}api/rolecapacity/history/${ideaId}`);
  }

  // 3. Playbook Facilitator
  public getPlaybookTemplates(): Observable<any[]> {
    return this.http.get<any[]>(`${this.baseUrl}api/sessionplaybook/templates`);
  }

  public advancePlaybookPhase(command: any): Observable<any> {
    return this.http.post(`${this.baseUrl}api/sessionplaybook/advance`, command);
  }

  // 4. AI Swarms
  public invokeAIAgent(command: any): Observable<any> {
    return this.http.post(`${this.baseUrl}api/aiagents/invoke`, command);
  }

  public getAIAgentInsights(ideaId: number, sessionId?: number): Observable<any[]> {
    const url = sessionId ? `${this.baseUrl}api/aiagents/insights/${ideaId}?sessionId=${sessionId}` : `${this.baseUrl}api/aiagents/insights/${ideaId}`;
    return this.http.get<any[]>(url);
  }

  public pinInsight(insightId: number): Observable<boolean> {
    return this.http.put<boolean>(`${this.baseUrl}api/aiagents/insights/${insightId}/pin`, {});
  }

  public triageIdea(ideaId: number): Observable<any> {
    return this.http.post(`${this.baseUrl}api/aiagents/triage/${ideaId}`, {});
  }

  // 5. 2D Spatial Canvas
  public getCanvasNodes(ideaId: number, sessionId?: number): Observable<any[]> {
    const url = sessionId ? `${this.baseUrl}api/ideacanvas/${ideaId}?sessionId=${sessionId}` : `${this.baseUrl}api/ideacanvas/${ideaId}`;
    return this.http.get<any[]>(url);
  }

  public saveCanvasNode(node: any): Observable<any> {
    return this.http.post(`${this.baseUrl}api/ideacanvas/node`, node);
  }

  public voteCanvasNode(ideaId: number, nodeId: number, increment: number = 1): Observable<number> {
    return this.http.post<number>(`${this.baseUrl}api/ideacanvas/node/${nodeId}/vote?ideaId=${ideaId}&increment=${increment}`, {});
  }

  public autoClusterCanvas(ideaId: number, sessionId?: number): Observable<any[]> {
    const url = sessionId ? `${this.baseUrl}api/ideacanvas/${ideaId}/cluster?sessionId=${sessionId}` : `${this.baseUrl}api/ideacanvas/${ideaId}/cluster`;
    return this.http.post<any[]>(url, {});
  }

  // 6. WebRTC Meeting & Speech Diarization
  public getMeetingToken(sessionId: number): Observable<any> {
    return this.http.get(`${this.baseUrl}api/livemeeting/token/${sessionId}`);
  }

  public extractSpeechActions(command: any): Observable<any> {
    return this.http.post(`${this.baseUrl}api/livemeeting/diarization/extract`, command);
  }

  // 7. Enterprise Connectors
  public getConnectors(ideaId: number): Observable<any[]> {
    return this.http.get<any[]>(`${this.baseUrl}api/connectors/${ideaId}`);
  }

  public configureConnector(command: any): Observable<any> {
    return this.http.post(`${this.baseUrl}api/connectors/configure`, command);
  }

  public syncActionToConnector(command: any): Observable<any> {
    return this.http.post(`${this.baseUrl}api/connectors/sync-action`, command);
  }

  // 8. Verifiable Provenance & W3C DIDs
  public generateCertificate(ideaId: number, issuerDid?: string): Observable<any> {
    const url = issuerDid ? `${this.baseUrl}api/provenance/certificate/${ideaId}?issuerDid=${encodeURIComponent(issuerDid)}` : `${this.baseUrl}api/provenance/certificate/${ideaId}`;
    return this.http.post(url, {});
  }

  public verifyProvenanceChain(ideaId: number): Observable<any> {
    return this.http.get(`${this.baseUrl}api/provenance/verify/${ideaId}`);
  }

  // 9. Innovation Economy & Quadratic Voting
  public castQuadraticVote(command: any): Observable<any> {
    return this.http.post(`${this.baseUrl}api/innovationeconomy/vote/quadratic`, command);
  }

  public placePrediction(command: any): Observable<any> {
    return this.http.post(`${this.baseUrl}api/innovationeconomy/prediction/place`, command);
  }

  public attachBounty(command: any): Observable<any> {
    return this.http.post(`${this.baseUrl}api/innovationeconomy/bounty/attach`, command);
  }

  // 10. Executive Analytics & Risk Matrix
  public getPipelineAnalytics(): Observable<any> {
    return this.http.get(`${this.baseUrl}api/outcomes/analytics`);
  }

  public getPortfolioRiskMatrix(): Observable<any> {
    return this.http.get(`${this.baseUrl}api/outcomes/risk-matrix`);
  }

  public recordOutcome(command: any): Observable<any> {
    return this.http.post(`${this.baseUrl}api/outcomes`, command);
  }

  // 11. Zero Trust ABAC & Blind Review
  public evaluateAccess(ideaId: number, department: string, clearance: string): Observable<any> {
    return this.http.get(`${this.baseUrl}api/zerotrustgovernance/evaluate-access/${ideaId}?department=${encodeURIComponent(department)}&clearance=${encodeURIComponent(clearance)}`);
  }

  public getAnonymizedIdea(ideaId: number): Observable<any> {
    return this.http.get(`${this.baseUrl}api/zerotrustgovernance/blind-review/idea/${ideaId}`);
  }

  public submitBlindReview(command: any): Observable<any> {
    return this.http.post(`${this.baseUrl}api/zerotrustgovernance/blind-review/submit`, command);
  }

  // 12. Edge Offline CRDT Sync
  public getCrdtSnapshot(ideaId: number): Observable<any> {
    return this.http.get(`${this.baseUrl}api/edgesync/snapshot/${ideaId}`);
  }

  public reconcileCrdt(command: any): Observable<any> {
    return this.http.post(`${this.baseUrl}api/edgesync/reconcile`, command);
  }
}
