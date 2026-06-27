import { useState } from 'react';
import type { FormEvent } from 'react';
import apiClient from '../api/apiClient';
import type { ApiResponse, TaskItem } from '../types';
import { TaskStatusLabel, PriorityLabel } from '../types';

interface Props {
  onTaskCreated: () => void;
  onClose: () => void;
}

export default function CreateTaskModal({ onTaskCreated, onClose }: Props) {
  const [title, setTitle] = useState('');
  const [description, setDescription] = useState('');
  const [status, setStatus] = useState('0');
  const [priority, setPriority] = useState('1');
  const [assignedTo, setAssignedTo] = useState('');
  const [errors, setErrors] = useState<string[]>([]);
  const [submitting, setSubmitting] = useState(false);

  async function handleSubmit(e: FormEvent) {
    e.preventDefault();
    setErrors([]);
    setSubmitting(true);
    try {
      const res = await apiClient.post<ApiResponse<TaskItem>>('/api/tasks', {
        title,
        description: description || undefined,
        status: Number(status),
        priority: Number(priority),
        assignedTo: assignedTo || undefined,
      });
      if (res.data.success) {
        onTaskCreated();
        onClose();
      } else {
        setErrors(res.data.errors?.length ? res.data.errors : [res.data.message]);
      }
    } catch (err: unknown) {
      const data = (err as { response?: { data?: ApiResponse<unknown> } })?.response?.data;
      setErrors(data?.errors?.length ? data.errors : [data?.message ?? 'Failed to create task.']);
    } finally {
      setSubmitting(false);
    }
  }

  return (
    <div className="modal-overlay" onClick={onClose}>
      <div className="modal-content card" onClick={(e) => e.stopPropagation()}>
        <div className="modal-header">
          <h2>New Task</h2>
          <button className="btn-icon" onClick={onClose} aria-label="Close">✕</button>
        </div>
        <form onSubmit={handleSubmit}>
          <div className="form-group">
            <label htmlFor="ct-title">Title *</label>
            <input
              id="ct-title"
              type="text"
              value={title}
              onChange={(e) => setTitle(e.target.value)}
              required
              maxLength={200}
            />
          </div>
          <div className="form-group">
            <label htmlFor="ct-desc">Description</label>
            <textarea
              id="ct-desc"
              value={description}
              onChange={(e) => setDescription(e.target.value)}
              rows={3}
            />
          </div>
          <div className="form-row">
            <div className="form-group">
              <label htmlFor="ct-status">Status</label>
              <select id="ct-status" value={status} onChange={(e) => setStatus(e.target.value)}>
                {Object.entries(TaskStatusLabel).map(([v, l]) => (
                  <option key={v} value={v}>{l}</option>
                ))}
              </select>
            </div>
            <div className="form-group">
              <label htmlFor="ct-priority">Priority</label>
              <select id="ct-priority" value={priority} onChange={(e) => setPriority(e.target.value)}>
                {Object.entries(PriorityLabel).map(([v, l]) => (
                  <option key={v} value={v}>{l}</option>
                ))}
              </select>
            </div>
          </div>
          <div className="form-group">
            <label htmlFor="ct-assigned">Assigned To</label>
            <input
              id="ct-assigned"
              type="text"
              value={assignedTo}
              onChange={(e) => setAssignedTo(e.target.value)}
              maxLength={100}
            />
          </div>
          {errors.length > 0 && (
            <ul className="error-list">
              {errors.map((e, i) => <li key={i}>{e}</li>)}
            </ul>
          )}
          <div className="modal-footer">
            <button type="button" className="btn btn-secondary" onClick={onClose}>Cancel</button>
            <button type="submit" className="btn btn-primary" disabled={submitting}>
              {submitting ? 'Creating…' : 'Create Task'}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
}
