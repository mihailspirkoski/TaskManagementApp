import { Component, signal } from '@angular/core';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';

import { CreateTaskDto } from '../../shared/models/task.model';
import { TaskService } from '../../shared/services/task.service';


@Component({
  selector: 'app-task-create',
  imports: [FormsModule, MatInputModule, MatButtonModule],
  templateUrl: './task-create.component.html',
  styleUrl: './task-create.component.scss'
})
export class TaskCreateComponent {

  dto = signal<CreateTaskDto>({title: '', description: '', dueDate: ''});

  constructor(private taskService: TaskService, private router: Router){}

  onSubmit(){
    this.taskService.createTask(this.dto()).subscribe({
      next: ()=> this.router.navigate(['/tasks']),
      error: (err) => console.error('Error creating task:', err)
    });
  }
}
