import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { AuthorizeGuard } from '../api-authorization/authorize.guard';
import { HomeComponent } from './home/home.component';
import { CounterComponent } from './counter/counter.component';
import { FetchDataComponent } from './fetch-data/fetch-data.component';
import { TodoComponent } from './todo/todo.component';
import { TokenComponent } from './token/token.component';
import { IdeaStudioComponent } from './idea-studio/idea-studio.component';
import { CanvasComponent } from './canvas/canvas.component';
import { WorkshopComponent } from './workshop/workshop.component';
import { AiSwarmComponent } from './ai-swarm/ai-swarm.component';
import { ExecutiveComponent } from './executive/executive.component';
import { GovernanceComponent } from './governance/governance.component';

export const routes: Routes = [
  { path: '', component: IdeaStudioComponent, pathMatch: 'full' },
  { path: 'idea-studio', component: IdeaStudioComponent },
  { path: 'canvas', component: CanvasComponent },
  { path: 'workshop', component: WorkshopComponent },
  { path: 'ai-swarm', component: AiSwarmComponent },
  { path: 'executive', component: ExecutiveComponent },
  { path: 'governance', component: GovernanceComponent },
  { path: 'counter', component: CounterComponent },
  { path: 'fetch-data', component: FetchDataComponent },
  { path: 'todo', component: TodoComponent, canActivate: [AuthorizeGuard] },
  { path: 'token', component: TokenComponent, canActivate: [AuthorizeGuard] }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule],
})
export class AppRoutingModule {}
