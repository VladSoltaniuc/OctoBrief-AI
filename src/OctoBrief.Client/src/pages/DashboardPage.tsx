import { useState, useEffect } from 'react'
import { useSearchParams, useNavigate } from 'react-router-dom'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { Octagon, ArrowLeft, Trash2, Power, PowerOff, RefreshCw, ExternalLink, Clock, Calendar } from 'lucide-react'
import { api } from '../api'
import { NotificationFrequency } from '../types'

export function DashboardPage() {
  const [searchParams] = useSearchParams()
  const navigate = useNavigate()
  const queryClient = useQueryClient()
  const [email, setEmail] = useState(searchParams.get('email') || '')

  useEffect(() => {
    if (!email) {
      const userEmail = prompt('Enter your email to view your dashboard:')
      if (!userEmail) {
        navigate('/')
        return
      }
      setEmail(userEmail)
    }
  }, [email, navigate])

  const { data: preferences = [], isLoading: preferencesLoading } = useQuery({
    queryKey: ['preferences', email],
    queryFn: () => api.getPreferences(email),
    enabled: !!email,
  })

  const { data: summaries = [], isLoading: summariesLoading } = useQuery({
    queryKey: ['summaries', email],
    queryFn: () => api.getSummaries(email),
    enabled: !!email,
  })

  const deleteMutation = useMutation({
    mutationFn: (id: number) => api.deletePreference(id),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['preferences'] })
      alert('✅ Monitoring preference deleted')
    },
    onError: (error: Error) => {
      alert(`❌ ${error.message}`)
    },
  })

  const toggleMutation = useMutation({
    mutationFn: ({ id, isActive }: { id: number; isActive: boolean }) =>
      api.updatePreference(id, { isActive }),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['preferences'] })
    },
    onError: (error: Error) => {
      alert(`❌ ${error.message}`)
    },
  })

  const triggerMutation = useMutation({
    mutationFn: (id: number) => api.triggerMonitoring(id),
    onSuccess: () => {
      alert('✅ Monitoring job queued! Check your email shortly.')
    },
    onError: (error: Error) => {
      alert(`❌ ${error.message}`)
    },
  })

  const getFrequencyText = (frequency: NotificationFrequency) => {
    switch (frequency) {
      case NotificationFrequency.Daily:
        return 'Daily'
      case NotificationFrequency.Weekly:
        return 'Weekly'
      case NotificationFrequency.Monthly:
        return 'Monthly'
      default:
        return 'Unknown'
    }
  }

  const getFrequencyIcon = (frequency: NotificationFrequency) => {
    switch (frequency) {
      case NotificationFrequency.Daily:
        return <Clock className="w-4 h-4" />
      case NotificationFrequency.Weekly:
      case NotificationFrequency.Monthly:
        return <Calendar className="w-4 h-4" />
      default:
        return <Clock className="w-4 h-4" />
    }
  }

  if (!email) return null

  return (
    <div className="min-h-screen bg-gradient-to-br from-primary-50 via-white to-blue-50">
      {/* Header */}
      <header className="border-b bg-white/80 backdrop-blur-sm sticky top-0 z-10">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-4">
          <div className="flex items-center justify-between">
            <div className="flex items-center gap-3">
              <Octagon className="w-8 h-8 text-primary-600" />
              <h1 className="text-2xl font-bold text-gray-900">OctoBrief</h1>
            </div>
            <button
              onClick={() => navigate('/')}
              className="text-primary-600 hover:text-primary-700 font-medium flex items-center gap-2"
            >
              <ArrowLeft className="w-4 h-4" />
              Back to Home
            </button>
          </div>
        </div>
      </header>

      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-12">
        {/* User Info */}
        <div className="mb-8">
          <h2 className="text-3xl font-bold text-gray-900 mb-2">Your Dashboard</h2>
          <p className="text-gray-600">Managing monitoring preferences for <span className="font-semibold">{email}</span></p>
        </div>

        {/* Monitoring Preferences */}
        <div className="mb-12">
          <h3 className="text-2xl font-bold text-gray-900 mb-6">Active Monitoring</h3>
          
          {preferencesLoading ? (
            <div className="flex items-center justify-center py-12">
              <div className="w-8 h-8 border-4 border-primary-600 border-t-transparent rounded-full animate-spin" />
            </div>
          ) : preferences.length === 0 ? (
            <div className="bg-white rounded-2xl p-12 text-center border border-gray-100">
              <p className="text-gray-600 mb-4">No monitoring preferences yet.</p>
              <button
                onClick={() => navigate('/')}
                className="text-primary-600 hover:text-primary-700 font-medium"
              >
                Create your first one →
              </button>
            </div>
          ) : (
            <div className="grid gap-6">
              {preferences.map((pref) => (
                <div
                  key={pref.id}
                  className={`bg-white rounded-2xl p-6 border transition ${
                    pref.isActive ? 'border-gray-200' : 'border-gray-100 opacity-60'
                  }`}
                >
                  <div className="flex items-start justify-between mb-4">
                    <div className="flex-1">
                      <div className="flex items-center gap-3 mb-2">
                        <h4 className="text-lg font-semibold text-gray-900">
                          {pref.websiteName || 'Unnamed Website'}
                        </h4>
                        <span className={`px-3 py-1 rounded-full text-xs font-medium flex items-center gap-1 ${
                          pref.isActive
                            ? 'bg-green-100 text-green-700'
                            : 'bg-gray-100 text-gray-600'
                        }`}>
                          {pref.isActive ? <Power className="w-3 h-3" /> : <PowerOff className="w-3 h-3" />}
                          {pref.isActive ? 'Active' : 'Paused'}
                        </span>
                      </div>
                      <a
                        href={pref.websiteUrl}
                        target="_blank"
                        rel="noopener noreferrer"
                        className="text-sm text-primary-600 hover:text-primary-700 flex items-center gap-1 mb-3"
                      >
                        {pref.websiteUrl}
                        <ExternalLink className="w-3 h-3" />
                      </a>
                      <div className="flex items-center gap-4 text-sm text-gray-600">
                        <span className="flex items-center gap-1">
                          {getFrequencyIcon(pref.frequency)}
                          {getFrequencyText(pref.frequency)}
                        </span>
                        {pref.lastProcessedAt && (
                          <span>
                            Last checked: {new Date(pref.lastProcessedAt).toLocaleDateString()}
                          </span>
                        )}
                      </div>
                    </div>
                    <div className="flex items-center gap-2">
                      <button
                        onClick={() => toggleMutation.mutate({ id: pref.id, isActive: !pref.isActive })}
                        className="p-2 hover:bg-gray-100 rounded-lg transition"
                        title={pref.isActive ? 'Pause monitoring' : 'Resume monitoring'}
                      >
                        {pref.isActive ? (
                          <PowerOff className="w-5 h-5 text-gray-600" />
                        ) : (
                          <Power className="w-5 h-5 text-gray-600" />
                        )}
                      </button>
                      <button
                        onClick={() => triggerMutation.mutate(pref.id)}
                        disabled={triggerMutation.isPending}
                        className="p-2 hover:bg-primary-50 rounded-lg transition text-primary-600"
                        title="Trigger now"
                      >
                        <RefreshCw className={`w-5 h-5 ${triggerMutation.isPending ? 'animate-spin' : ''}`} />
                      </button>
                      <button
                        onClick={() => {
                          if (confirm('Are you sure you want to delete this monitoring preference?')) {
                            deleteMutation.mutate(pref.id)
                          }
                        }}
                        className="p-2 hover:bg-red-50 rounded-lg transition text-red-600"
                        title="Delete"
                      >
                        <Trash2 className="w-5 h-5" />
                      </button>
                    </div>
                  </div>
                </div>
              ))}
            </div>
          )}
        </div>

        {/* Recent Summaries */}
        <div>
          <h3 className="text-2xl font-bold text-gray-900 mb-6">Recent Summaries</h3>
          
          {summariesLoading ? (
            <div className="flex items-center justify-center py-12">
              <div className="w-8 h-8 border-4 border-primary-600 border-t-transparent rounded-full animate-spin" />
            </div>
          ) : summaries.length === 0 ? (
            <div className="bg-white rounded-2xl p-12 text-center border border-gray-100">
              <p className="text-gray-600">No summaries yet. They'll appear here once monitoring runs.</p>
            </div>
          ) : (
            <div className="grid gap-6">
              {summaries.map((summary) => (
                <div key={summary.id} className="bg-white rounded-2xl p-6 border border-gray-200">
                  <div className="mb-4">
                    <div className="flex items-center justify-between mb-2">
                      <h4 className="text-lg font-semibold text-gray-900">{summary.websiteName}</h4>
                      <span className="text-sm text-gray-500">
                        {new Date(summary.createdAt).toLocaleDateString()}
                      </span>
                    </div>
                    <p className="text-gray-700 leading-relaxed">{summary.summary}</p>
                  </div>

                  {summary.headlines.length > 0 && (
                    <div className="space-y-3 pt-4 border-t border-gray-100">
                      {summary.headlines.map((headline, idx) => (
                        <div key={idx} className="bg-gray-50 rounded-xl p-4">
                          <h5 className="font-medium text-gray-900 mb-1">{headline.title}</h5>
                          <p className="text-sm text-gray-600 mb-2">{headline.summary}</p>
                          <a
                            href={headline.sourceUrl}
                            target="_blank"
                            rel="noopener noreferrer"
                            className="text-sm text-primary-600 hover:text-primary-700 flex items-center gap-1"
                          >
                            Read more
                            <ExternalLink className="w-3 h-3" />
                          </a>
                        </div>
                      ))}
                    </div>
                  )}
                </div>
              ))}
            </div>
          )}
        </div>
      </div>
    </div>
  )
}
