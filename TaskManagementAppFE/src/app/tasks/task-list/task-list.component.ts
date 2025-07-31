import { Component, effect, Signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatListModule } from '@angular/material/list';

import { TaskService } from '../../shared/services/task.service';
import { Task } from '../../shared/models/task.model';



@Component({
  selector: 'app-task-list',
  imports: [RouterLink, MatButtonModule, MatListModule],
  templateUrl: './task-list.component.html',
  styleUrl: './task-list.component.scss'
})
export class TaskListComponent {

  tasks!: Signal<Task[]>;

  constructor(private taskService: TaskService){
    this.tasks = this.taskService!.getTasksSignal();
    effect(() => {
      this.taskService.getTasks().subscribe({
        next: (tasks) => this.taskService.setTasks(tasks),
        error: (err) => console.error('Error fetching tasks:', err)
     });
   });
  }
}