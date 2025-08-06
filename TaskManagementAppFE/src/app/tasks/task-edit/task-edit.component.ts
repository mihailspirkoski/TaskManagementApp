import { Component, signal } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatInputModule } from '@angular/material/input';
import { MatCheckboxModule } from '@angular/material/checkbox';

import { CreateTaskDto, Task, UpdateTaskDto } from '../../shared/models/task.model';
import { TaskService } from '../../shared/services/task.service';


@Component({
  selector: 'app-task-edit',
  imports: [FormsModule, MatButtonModule, MatInputModule, MatCheckboxModule],
  templateUrl: './task-edit.component.html',
  styleUrl: './task-edit.component.scss'
})
export class TaskEditComponent {

  task = signal<UpdateTaskDto>({id: 0, title: '', description: '', dueDate: '', isCompleted: false});
  //dto = signal<CreateTaskDto>({title: '', description: '', dueDate: ''});

  constructor(private taskService: TaskService, private route: ActivatedRoute, private router: Router){}

  ngOnInit(){
    const id = this.route.snapshot.paramMap.get('id');
    if(id){
      this.taskService.getTaskById(+id).subscribe({
        next: (task) => {
          this.task.set(task);
        },
        error: (err) => console.error('Failed to load task:', err)
      });
    }
  }

  onSubmit(){
    const id = this.route.snapshot.paramMap.get('id');
    if(id){
      this.taskService.updateTask(this.task()).subscribe({
        next: () => this.router.navigate(['tasks']),
        error: (err) => console.error('Error updating task:', err)
      });
    }
  }

  cancel(){
    this.router.navigate(['/tasks']);
  }
  
}
