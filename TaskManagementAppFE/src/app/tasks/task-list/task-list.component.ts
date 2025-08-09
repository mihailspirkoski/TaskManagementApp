import { Component, effect, Signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatListModule } from '@angular/material/list';
import { MatDialog } from '@angular/material/dialog';

import { TaskService } from '../../shared/services/task.service';
import { Task } from '../../shared/models/task.model';
import { ConfirmDialogComponent } from '../../shared/components/confirm-dialog/confirm-dialog.component';



@Component({
  selector: 'app-task-list',
  imports: [RouterLink, MatButtonModule, MatListModule],
  templateUrl: './task-list.component.html',
  styleUrl: './task-list.component.scss'
})
export class TaskListComponent {

  tasks!: Signal<Task[]>;

  constructor(private taskService: TaskService, private dialog: MatDialog) {
    this.tasks = this.taskService!.getTasksSignal();
    effect(() => {
      this.taskService.getTasks().subscribe({
        next: (tasks) => this.taskService.setTasks(tasks),
        error: (err) => console.error('Error fetching tasks:', err)
     });
   });
  }

  deleteTask(task: Task) {
    const dialogRef = this.dialog.open(ConfirmDialogComponent, {
      data: {title: task.title}
    });
    dialogRef.afterClosed().subscribe(result => {
      if(result){
        this.taskService.deleteTask(task.id).subscribe({
          next: () => {
            this.taskService.getTasks().subscribe({
              next: (tasks) => this.taskService.setTasks(tasks),
              error: (err) => console.error('Error refreshing tasks:', err)
            });
          },
          error: (err) => console.error('Error deleting task:', err)
        });
      }
    });
  }
}