import React, { useState, useEffect } from 'react';
import { auditLogsV2API, getErrorMessage } from '../lib/api';
import AuditTimeline from '../components/auditv2/AuditTimeline';
import AuditDetailPanel from '../components/auditv2/AuditDetailPanel';
import AuditFilterSidebar from '../components/auditv2/AuditFilterSidebar';

const AuditLogsV2 = () => {
  const [timelineData, setTimelineData] = useState([]);
  const [selectedLogDetail, setSelectedLogDetail] = useState(null);
  const [selectedLogId, setSelectedLogId] = useState(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);
  const [appliedFilters, setAppliedFilters] = useState(null);

  useEffect(() => {
    fetchTimeline();
  }, []);

  const fetchTimeline = async (fromDate = null, toDate = null) => {
    try {
      setLoading(true);
      setError(null);
      const response = await auditLogsV2API.getTimeline(fromDate, toDate);
      setTimelineData(response.data);
    } catch (err) {
      setError(getErrorMessage(err));
      console.error('Error fetching audit timeline:', err);
    } finally {
      setLoading(false);
    }
  };

  const fetchLogDetail = async (logId) => {
    try {
      const response = await auditLogsV2API.getDetail(logId);
      setSelectedLogDetail(response.data);
      setSelectedLogId(logId);
    } catch (err) {
      setError(getErrorMessage(err));
      console.error('Error fetching log detail:', err);
    }
  };

  const handleCheckpointClick = (logId) => {
    if (selectedLogId === logId) {
      // Close panel if clicking same item
      setSelectedLogId(null);
      setSelectedLogDetail(null);
    } else {
      fetchLogDetail(logId);
    }
  };

  const handleFilter = async (filters) => {
    try {
      setLoading(true);
      setError(null);
      setAppliedFilters(filters);
      
      const response = await auditLogsV2API.filter(filters);
      
      // Group by date manually (since filter endpoint doesn't return grouped data)
      const grouped = response.data.reduce((acc, log) => {
        const date = new Date(log.timestamp).toISOString().split('T')[0];
        const existingGroup = acc.find(g => g.date === date);
        
        if (existingGroup) {
          existingGroup.logs.push({
            id: log.id,
            message: log.message,
            timestamp: log.timestamp,
            userName: log.userName,
            entityName: log.entityName,
            actionType: log.actionType
          });
        } else {
          acc.push({
            date,
            logs: [{
              id: log.id,
              message: log.message,
              timestamp: log.timestamp,
              userName: log.userName,
              entityName: log.entityName,
              actionType: log.actionType
            }]
          });
        }
        
        return acc;
      }, []);
      
      // Sort by date descending
      grouped.sort((a, b) => new Date(b.date) - new Date(a.date));
      
      setTimelineData(grouped);
    } catch (err) {
      setError(getErrorMessage(err));
      console.error('Error filtering audit logs:', err);
    } finally {
      setLoading(false);
    }
  };

  const handleClearFilter = () => {
    setAppliedFilters(null);
    fetchTimeline();
  };

  const closeDetailPanel = () => {
    setSelectedLogId(null);
    setSelectedLogDetail(null);
  };

  return (
    <div className="h-screen flex bg-gray-50">
      {/* Filter Sidebar */}
      <AuditFilterSidebar 
        onFilter={handleFilter}
        onClear={handleClearFilter}
      />

      {/* Main Content */}
      <div className="flex-1 flex flex-col overflow-hidden">
        {/* Header */}
        <div className="bg-white border-b border-gray-200 px-8 py-6">
          <div className="flex items-center justify-between">
            <div>
              <h1 className="text-3xl font-bold text-gray-900">Audit Logs V2</h1>
              <p className="text-gray-500 mt-1">Timeline view of all system activities</p>
            </div>
            <div className="flex items-center gap-2">
              <span className="px-3 py-1 bg-blue-100 text-blue-800 rounded-full text-sm font-medium">
                Timeline View
              </span>
              {appliedFilters && (
                <span className="px-3 py-1 bg-green-100 text-green-800 rounded-full text-sm font-medium">
                  Filtered
                </span>
              )}
            </div>
          </div>
        </div>

        {/* Timeline View */}
        <div className="flex-1 overflow-y-auto px-8 py-6">
          {error && (
            <div className="mb-6 bg-red-50 border border-red-200 text-red-700 px-4 py-3 rounded-lg">
              {error}
            </div>
          )}

          {loading ? (
            <div className="flex items-center justify-center h-64">
              <div className="flex flex-col items-center gap-4">
                <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600"></div>
                <p className="text-gray-500">Loading audit logs...</p>
              </div>
            </div>
          ) : (
            <AuditTimeline 
              timelineData={timelineData}
              onCheckpointClick={handleCheckpointClick}
              selectedLogId={selectedLogId}
            />
          )}
        </div>
      </div>

      {/* Detail Panel (Slide-in from right) */}
      {selectedLogDetail && (
        <>
          {/* Backdrop */}
          <div 
            className="fixed inset-0 bg-black bg-opacity-30 z-40"
            onClick={closeDetailPanel}
          ></div>
          
          {/* Panel */}
          <AuditDetailPanel 
            detail={selectedLogDetail}
            onClose={closeDetailPanel}
          />
        </>
      )}
    </div>
  );
};

export default AuditLogsV2;
