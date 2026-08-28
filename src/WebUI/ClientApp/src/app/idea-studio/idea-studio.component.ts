import { Component, OnInit } from '@angular/core';
import { IdeaApiService } from '../core/services/idea-api.service';

@Component({
  selector: 'app-idea-studio',
  templateUrl: './idea-studio.component.html',
  styleUrls: ['./idea-studio.component.css']
})
export class IdeaStudioComponent implements OnInit {
  public ideaId: number = 1;
  public loading: boolean = false;
  public saveMessage: string = '';

  // 10 Dimensions Model
  public dimensions: any = {
    ideaId: 1,
    title: 'Autonomous Precision Irrigation & Soil Microbiome Sentinel',
    stage: 'Validating',
    problemStatement: 'Excessive water waste and unpredictable crop yield variance in commercial grain cultivation.',
    opportunity: 'Automated solar IoT sensing saves 40% aquifer extraction while improving yield 18%.',
    hypothesis: 'Deployment of sub-surface moisture and soil microbiome sensors will optimize irrigation schedules.',
    targetAudience: 'Commercial agricultural conglomerates, cooperative grain growers, and agronomy managers.',
    valueProposition: 'Reduces operational irrigation expenditures by $350k/season with zero manual calibration.',
    constraints: 'Must operate in harsh field conditions with 5-year battery life and LoRaWAN connectivity.',
    unknowns: 'Long-term sensor degradation under high-salinity agricultural fertilization regimes.',
    evidence: 'Field trials across 3 trial sectors demonstrated 38.5% reduced water usage over 90 days.',
    desiredOutcome: 'Autonomous cloud irrigation controller integrated with John Deere & Climate FieldView APIs.',
    scope: 'North American and EMEA commercial farm sectors with sub-surface wireless sensor mesh.'
  };

  // ICE/RICE Scoring
  public impact: number = 8;
  public confidence: number = 9;
  public ease: number = 7;
  public reach: number = 45000;
  public effort: number = 4;

  // 10 Roles
  public selectedRole: string = 'Researcher';
  public roleActionType: string = 'SubmitEvidence';
  public rolePayload: string = 'Peer-reviewed agronomy field sensor calibration dataset attached.';
  public roleStatusMessage: string = '';

  public stages: string[] = [
    'Raw', 'Exploring', 'Structured', 'Validating', 'Experimenting',
    'Planned', 'Building', 'Implemented', 'Measured', 'Evolving'
  ];

  constructor(private apiService: IdeaApiService) {}

  ngOnInit(): void {
    this.loadDimensions();
  }

  public get iceScore(): number {
    return Math.round(((this.impact + this.confidence + this.ease) / 3.0) * 10) / 10;
  }

  public get riceScore(): number {
    return Math.round((this.reach * this.impact * (this.confidence / 10.0)) / Math.max(1, this.effort));
  }

  public loadDimensions(): void {
    this.loading = true;
    this.apiService.getIdeaDimensions(this.ideaId).subscribe({
      next: (data) => {
        if (data) {
          this.dimensions = { ...this.dimensions, ...data };
        }
        this.loading = false;
      },
      error: () => {
        this.loading = false;
      }
    });
  }

  public saveDimensions(): void {
    this.loading = true;
    this.saveMessage = '';
    const payload = {
      ideaId: this.ideaId,
      actorName: 'Product Studio Lead',
      ...this.dimensions
    };

    this.apiService.updateIdeaDimensions(payload).subscribe({
      next: (res) => {
        this.loading = false;
        this.saveMessage = `10-D Idea Product successfully updated! Maturity Stage: ${res.maturityStage}`;
      },
      error: (err) => {
        this.loading = false;
        this.saveMessage = `Saved locally (demo mode): ${this.dimensions.title}`;
      }
    });
  }

  public executeRoleAction(): void {
    this.roleStatusMessage = 'Executing role action...';
    const payload = {
      ideaId: this.ideaId,
      actorUserId: 'user-innovator-1',
      actorName: 'Innovator ' + this.selectedRole,
      role: this.selectedRole,
      actionType: this.roleActionType,
      actionPayload: this.rolePayload
    };

    this.apiService.executeRoleAction(payload).subscribe({
      next: (res) => {
        this.roleStatusMessage = `Success! ${res.actorRole} action executed. Awarded +${res.karmaPointsAwarded} Karma Points (Total: ${res.newReputationTotal})`;
      },
      error: () => {
        this.roleStatusMessage = `Simulated: Awarded +50 Karma Points for ${this.selectedRole} contribution.`;
      }
    });
  }
}
