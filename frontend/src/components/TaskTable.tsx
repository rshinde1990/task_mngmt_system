import apiClient from '../api/apiClient';
import type { TaskItem } from '../types';
import { TaskStatusLabel, PriorityLabel, PriorityColor } from '../types';

interface Props {
  tasks: TaskItem[];
  onRefresh: () => void;
}

export default function TaskTable({ tasks, onRefresh }: Props) {
  async function handleStatusChange(id: number, newStatus: string) {
    await apiClient.put(`/api/tasks/${id}`, { status: Number(newStatus) });
    onRefresh();
  }

  async function handleDelete(id: number, title: string) {
    if (!window.confirm(`Delete "${title}"?`)) return;
    await apiClient.delete(`/api/tasks/${id}`);
    onRefresh();
  }

  if (tasks.length === 0) {
    return <p className="empty-state">No tasks found. Create one to get started.</p>;
  }

  return (
    <div className="table-wrapper">
      <table className="task-table">
        <thead>
          <tr>
            <th>Title</th>
            <th>Status</th>
            <th>Priority</th>
            <th>Assigned To</th>
            <th>Actions</th>
          </tr>
        </thead>
        <tbody>
          {tasks.map((task) => (
            <tr key={task.id} style={{ backgroundColor: PriorityColor[task.priority] }}>
              <td>
                <span className="task-title">{task.title}</span>
                {task.description && (
                  <span className="task-desc">{task.description}</span>
                )}
              </td>
              <td>
                <select
                  value={task.status}
                  onChange={(e) => handleStatusChange(task.id, e.target.value)}
                  className="inline-select"
                >
                  {Object.entries(TaskStatusLabel).map(([v, l]) => (
                    <option key={v} value={v}>{l}</option>
                  ))}
                </select>
              </td>
              <td>{PriorityLabel[task.priority] ?? task.priority}</td>
              <td>{task.assignedTo ?? '—'}</td>
              <td>
                <button
                  className="btn btn-danger btn-sm"
                  onClick={() => handleDelete(task.id, task.title)}
                >
                  Delete
                </button>
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
}
