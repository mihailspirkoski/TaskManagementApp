export interface Task {
    id: number;
    title: string;
    description?: string;
    dueDate?: string;
    isCompleted: boolean;
}

export interface CreateTaskDto{
    title: string;
    description?: string;
    dueDate?: string;
}

export interface UpdateTaskDto{
    id: number;
    title: string;
    description?: string;
    dueDate?: string;
    isCompleted?: boolean;
}