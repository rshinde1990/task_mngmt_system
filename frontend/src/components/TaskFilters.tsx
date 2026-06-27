import { TaskStatusLabel, PriorityLabel } from '../types';

interface Props {
  status: string;
  priority: string;
  onFilterChange: (key: 'status' | 'priority', value: string) => void;
}

export default function TaskFilters({ status, priority, onFilterChange }: Props) {
  return (
    <div className="filters">
      <div className="form-group-inline">
        <label htmlFor="filter-status">Status</label>
        <select
          id="filter-status"
          value={status}
          onChange={(e) => onFilterChange('status', e.target.value)}
        >
          <option value="">All</option>
          {Object.entries(TaskStatusLabel).map(([val, label]) => (
            <option key={val} value={val}>{label}</option>
          ))}
        </select>
      </div>
      <div className="form-group-inline">
        <label htmlFor="filter-priority">Priority</label>
        <select
          id="filter-priority"
          value={priority}
          onChange={(e) => onFilterChange('priority', e.target.value)}
        >
          <option value="">All</option>
          {Object.entries(PriorityLabel).map(([val, label]) => (
            <option key={val} value={val}>{label}</option>
          ))}
        </select>
      </div>
    </div>
  );
}
