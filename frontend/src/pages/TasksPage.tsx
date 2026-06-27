import { useEffect, useState, useCallback } from 'react';
import { useNavigate } from 'react-router-dom';
import apiClient from '../api/apiClient';
import type { ApiResponse, TaskItem } from '../types';
import TaskFilters from '../components/TaskFilters';
import TaskTable from '../components/TaskTable';
import CreateTaskModal from '../components/CreateTaskModal';
import SummaryPanel from '../components/SummaryPanel';

export default function TasksPage() {
  const navigate = useNavigate();
  const [tasks, setTasks] = useState<TaskItem[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [statusFilter, setStatusFilter] = useState('');
  const [priorityFilter, setPriorityFilter] = useState('');
  const [showModal, setShowModal] = useState(false);

  const fetchTasks = useCallback(async () => {
    setLoading(true);
    setError('');
    try {
      const params = new URLSearchParams();
      if (statusFilter) params.set('status', statusFilter);
      if (priorityFilter) params.set('priority', priorityFilter);
      const res = await apiClient.get<ApiResponse<TaskItem[]>>(
        `/api/tasks?${params.toString()}`
      );
      setTasks(res.data.data ?? []);
    } catch (err: unknown) {
      const status = (err as { response?: { status?: number } })?.response?.status;
      if (status === 401) {
        localStorage.removeItem('token');
        navigate('/login');
      } else {
        setError('Failed to load tasks.');
      }
    } finally {
      setLoading(false);
    }
  }, [statusFilter, priorityFilter, navigate]);

  useEffect(() => {
    fetchTasks();
  }, [fetchTasks]);

  function handleFilterChange(key: 'status' | 'priority', value: string) {
    if (key === 'status') setStatusFilter(value);
    else setPriorityFilter(value);
  }

  function handleLogout() {
    localStorage.removeItem('token');
    navigate('/login');
  }

  return (
    <div className="page-container">
      <header className="page-header">
        <h1>Task Management</h1>
        <button className="btn btn-secondary" onClick={handleLogout}>Logout</button>
      </header>

      <SummaryPanel />

      <div className="card tasks-card">
        <div className="tasks-toolbar">
          <TaskFilters
            status={statusFilter}
            priority={priorityFilter}
            onFilterChange={handleFilterChange}
          />
          <button className="btn btn-primary" onClick={() => setShowModal(true)}>
            + New Task
          </button>
        </div>

        {loading && <div className="spinner">Loading tasks…</div>}
        {error && <p className="error-text">{error}</p>}
        {!loading && !error && (
          <TaskTable tasks={tasks} onRefresh={fetchTasks} />
        )}
      </div>

      {showModal && (
        <CreateTaskModal
          onTaskCreated={fetchTasks}
          onClose={() => setShowModal(false)}
        />
      )}
    </div>
  );
}
