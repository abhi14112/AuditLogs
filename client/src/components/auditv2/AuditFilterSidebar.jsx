import React, { useState } from 'react';

const AuditFilterSidebar = ({ onFilter, onClear }) => {
  const [filters, setFilters] = useState({
    fromDate: '',
    toDate: '',
    entityName: '',
  });

  const handleInputChange = (e) => {
    const { name, value } = e.target;
    setFilters(prev => ({
      ...prev,
      [name]: value
    }));
  };

  const handleApplyFilter = () => {
    onFilter(filters);
  };

  const handleClearFilter = () => {
    setFilters({
      fromDate: '',
      toDate: '',
      entityName: '',
    });
    onClear();
  };

  return (
    <div className="w-64 bg-white border-r border-gray-200 overflow-y-auto flex-shrink-0">
      <div className="p-6 space-y-6">
      <div>
        <h3 className="text-lg font-semibold text-gray-900 mb-4">Filters</h3>
      </div>

      {/* Date Range Filter */}
      <div>
        <label className="block text-sm font-medium text-gray-700 mb-2">
          From Date
        </label>
        <input
          type="date"
          name="fromDate"
          value={filters.fromDate}
          onChange={handleInputChange}
          className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
        />
      </div>

      <div>
        <label className="block text-sm font-medium text-gray-700 mb-2">
          To Date
        </label>
        <input
          type="date"
          name="toDate"
          value={filters.toDate}
          onChange={handleInputChange}
          className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
        />
      </div>

      {/* Entity Filter */}
      <div>
        <label className="block text-sm font-medium text-gray-700 mb-2">
          Entity Type
        </label>
        <select
          name="entityName"
          value={filters.entityName}
          onChange={handleInputChange}
          className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
        >
          <option value="">All Entities</option>
          <option value="Product">Product</option>
          <option value="Inventory">Inventory</option>
          <option value="User">User</option>
        </select>
      </div>

      {/* Action Buttons */}
      <div className="space-y-2 pt-4">
        <button
          onClick={handleApplyFilter}
          className="w-full bg-blue-600 text-white py-2 px-4 rounded-lg hover:bg-blue-700 transition-colors font-medium"
        >
          Apply Filters
        </button>
        <button
          onClick={handleClearFilter}
          className="w-full bg-gray-100 text-gray-700 py-2 px-4 rounded-lg hover:bg-gray-200 transition-colors font-medium"
        >
          Clear Filters
        </button>
      </div>

      {/* Quick Filters */}
      <div className="pt-6 border-t border-gray-200">
        <h4 className="text-sm font-semibold text-gray-700 mb-3">Quick Filters</h4>
        <div className="space-y-2">
          <button
            onClick={() => {
              const today = new Date().toISOString().split('T')[0];
              const newFilters = { ...filters, fromDate: today, toDate: today };
              setFilters(newFilters);
              onFilter(newFilters);
            }}
            className="w-full text-left text-sm text-gray-600 hover:text-blue-600 hover:bg-blue-50 px-3 py-2 rounded-lg transition-colors"
          >
            Today
          </button>
          <button
            onClick={() => {
              const today = new Date();
              const lastWeek = new Date(today.getTime() - 7 * 24 * 60 * 60 * 1000);
              const newFilters = {
                ...filters,
                fromDate: lastWeek.toISOString().split('T')[0],
                toDate: today.toISOString().split('T')[0]
              };
              setFilters(newFilters);
              onFilter(newFilters);
            }}
            className="w-full text-left text-sm text-gray-600 hover:text-blue-600 hover:bg-blue-50 px-3 py-2 rounded-lg transition-colors"
          >
            Last 7 Days
          </button>
          <button
            onClick={() => {
              const today = new Date();
              const lastMonth = new Date(today.getTime() - 30 * 24 * 60 * 60 * 1000);
              const newFilters = {
                ...filters,
                fromDate: lastMonth.toISOString().split('T')[0],
                toDate: today.toISOString().split('T')[0]
              };
              setFilters(newFilters);
              onFilter(newFilters);
            }}
            className="w-full text-left text-sm text-gray-600 hover:text-blue-600 hover:bg-blue-50 px-3 py-2 rounded-lg transition-colors"
          >
            Last 30 Days
          </button>
        </div>
      </div>
    </div>
    </div>
  );
};

export default AuditFilterSidebar;
