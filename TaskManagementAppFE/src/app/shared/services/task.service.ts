import { Injectable, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';

import { CreateTaskDto, Task } from '../models/task.model';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class TaskService {

  private apiUrl = 'http://localhost:5179/api/task';
  private tasks = signal<Task[]>([]);

  constructor(private http: HttpClient) { }

  getTasksSignal(){
    return this.tasks.asReadonly();
  }

    setTasks(tasks: Task[]){
      this.tasks.set(tasks);
    }

  getTasks(): Observable<Task[]>{
    return this.http.get<Task[]>(`${this.apiUrl}/user`);
  }

  createTask(dto: CreateTaskDto): Observable<Task>{
    return this.http.post<Task>(this.apiUrl, dto);
  }


}
