import { Injectable, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';

import { CreateTaskDto, Task, UpdateTaskDto } from '../models/task.model';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class TaskService {

  private apiUrl = 'https://localhost:7298/api/task';
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
  
  getTaskById(id: number): Observable<UpdateTaskDto>{
    return this.http.get<UpdateTaskDto>(`${this.apiUrl}/${id}`);
  }
  createTask(dto: CreateTaskDto): Observable<Task>{
    return this.http.post<Task>(this.apiUrl, dto);
  }

  updateTask(task: UpdateTaskDto): Observable<void>{
      return this.http.put<void>(this.apiUrl, task);
  }

  deleteTask(id: number) : Observable<void>{
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
