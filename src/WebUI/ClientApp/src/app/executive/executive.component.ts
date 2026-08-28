import { Component, OnInit } from '@angular/core';
import { IdeaApiService } from '../core/services/idea-api.service';

@Component({
  selector: 'app-executive',
  templateUrl: './executive.component.html',
  styleUrls: ['./executive.component.css']
})
export class ExecutiveComponent implements OnInit {
  public analytics: any = {
    totalIdeas: 24,
    ideaToOutcomeConversionRate: 18.5,
    totalEstimatedCostSavings: 2450000,
    totalRevenueGenerated: 1800000,
    totalImpactedUsers: 45000,
    averageTimeToFirstActionDays: 1.8,
    rawCount: 6,
    exploringCount: 4,
    structuredCount: 4,
    validatingCount: 3,
    experimentingCount: 2,
    plannedCount: 2,
    buildingCount: 1,
    implementedCount: 1,
    measuredCount: 1,
    evolvingCount: 0
  };

  public riskMatrix: any = {
    quickWins: [
      { ideaId: 1, title: 'LoRaWAN Moisture Sensor Mesh', impactScore: 8.8, complexityScore: 3.5, category: 'Agritech', stage: 'Validating' },
      { ideaId: 4, title: 'Mobile Offline Field Dispatch PWA', impactScore: 8.2, complexityScore: 4.1, category: 'Operations', stage: 'Planned' }
    ],
    strategicBets: [
      { ideaId: 2, title: 'AI Microbiome Crop Yield Engine', impactScore: 9.4, complexityScore: 8.0, category: 'Agritech', stage: 'Experimenting' },
      { ideaId: 6, title: 'Autonomous Micro-Drone Pollination Grid', impactScore: 8.9, complexityScore: 7.8, category: 'Robotics', stage: 'Exploring' }
    ],
    lowHangingFruit: [
      { ideaId: 3, title: 'SMS Irrigation Threshold Alerts', impactScore: 6.2, complexityScore: 2.2, category: 'Utilities', stage: 'Implemented' }
    ],
    complexInitiatives: [
      { ideaId: 5, title: 'Satellite Multi-Spectral Soil Radar', impactScore: 6.0, complexityScore: 8.5, category: 'Aerospace', stage: 'Raw' }
    ]
  };

  public newOutcome = {
    ideaId: 1,
    title: 'Phase 1 Sensor Trial Completion',
    summary: 'Demonstrated 38.5% reduced water usage across 3 trial quadrants',
    estimatedCostSavings: 350000,
    revenueGenerated: 120000,
    impactedUsersCount: 14000,
    estimatedRoiPercent: 190
  };
  public outcomeRecordedMsg: string = '';

  constructor(private apiService: IdeaApiService) {}

  ngOnInit(): void {
    this.loadAnalytics();
  }

  public loadAnalytics(): void {
    this.apiService.getPipelineAnalytics().subscribe({
      next: (data) => {
        if (data) this.analytics = data;
      }
    });

    this.apiService.getPortfolioRiskMatrix().subscribe({
      next: (matrix) => {
        if (matrix && matrix.quickWins) this.riskMatrix = matrix;
      }
    });
  }

  public recordDeliveredOutcome(): void {
    this.apiService.recordOutcome(this.newOutcome).subscribe({
      next: (res) => {
        this.outcomeRecordedMsg = `Delivered outcome recorded! Idea maturity stage advanced to Measured. Realized ROI: ${res.estimatedRoiPercent}%`;
        this.analytics.totalEstimatedCostSavings += res.estimatedCostSavings;
      },
      error: () => {
        this.outcomeRecordedMsg = `Simulated: Recorded $350k cost savings outcome. Stage advanced to Measured.`;
        this.analytics.totalEstimatedCostSavings += 350000;
      }
    });
  }
}
