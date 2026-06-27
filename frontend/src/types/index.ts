export interface ApiResponse<T> {
  success: boolean;
  data: T;
  message: string;
  errors: string[];
  statusCode: number;
}

export interface TaskItem {
  id: number;
  title: string;
  description: string | null;
  status: number;
  priority: number;
  assignedTo: string | null;
  createdDate: string;
  modifiedDate: string;
  isDeleted: boolean;
}

export interface CreateTaskDto {
  title: string;
  description?: string;
  status: number;
  priority: number;
  assignedTo?: string;
}

export interface UpdateTaskDto {
  title?: string;
  description?: string;
  status?: number;
  priority?: number;
  assignedTo?: string;
}

export interface TaskSummary {
  status: number;
  priority: number;
  count: number;
}

export const TaskStatusLabel: Record<number, string> = {
  0: 'ToDo',
  1: 'InProgress',
  2: 'Done',
};

export const PriorityLabel: Record<number, string> = {
  0: 'Low',
  1: 'Medium',
  2: 'High',
  3: 'Critical',
};

export const PriorityColor: Record<number, string> = {
  3: '#ffcccc',
  2: '#ffe0b2',
  1: '#fff9c4',
  0: '#c8e6c9',
};
