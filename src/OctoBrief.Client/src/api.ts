import {
  CreatePreferenceDto,
  MonitoringPreference,
  WebsiteSummary,
  ConfigStatus,
  ScrapeTestResult,
  AiTestResult,
  EmailTestResult,
  FullTestResult,
  PreviewBriefResponse,
  SendEmailResponse,
} from './types'

const API_BASE = '/api'

export const api = {
  // Preferences
  async getPreferences(email?: string): Promise<MonitoringPreference[]> {
    const url = email ? `${API_BASE}/preferences?email=${encodeURIComponent(email)}` : `${API_BASE}/preferences`
    const response = await fetch(url)
    if (!response.ok) throw new Error('Failed to fetch preferences')
    return response.json()
  },

  async createPreference(data: CreatePreferenceDto): Promise<MonitoringPreference> {
    const response = await fetch(`${API_BASE}/preferences`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(data),
    })
    if (!response.ok) {
      const error = await response.json()
      throw new Error(error.message || 'Failed to create preference')
    }
    return response.json()
  },

  async deletePreference(id: number): Promise<void> {
    const response = await fetch(`${API_BASE}/preferences/${id}`, {
      method: 'DELETE',
    })
    if (!response.ok) throw new Error('Failed to delete preference')
  },

  async updatePreference(id: number, data: Partial<MonitoringPreference>): Promise<MonitoringPreference> {
    const response = await fetch(`${API_BASE}/preferences/${id}`, {
      method: 'PUT',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(data),
    })
    if (!response.ok) throw new Error('Failed to update preference')
    return response.json()
  },

  async triggerMonitoring(id: number): Promise<void> {
    const response = await fetch(`${API_BASE}/preferences/${id}/trigger`, {
      method: 'POST',
    })
    if (!response.ok) throw new Error('Failed to trigger monitoring')
  },

  // Summaries
  async getSummaries(email?: string): Promise<WebsiteSummary[]> {
    const url = email ? `${API_BASE}/summaries?email=${encodeURIComponent(email)}` : `${API_BASE}/summaries`
    const response = await fetch(url)
    if (!response.ok) throw new Error('Failed to fetch summaries')
    return response.json()
  },

  async previewSummary(url: string): Promise<WebsiteSummary> {
    const response = await fetch(`${API_BASE}/summaries/preview`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ url }),
    })
    if (!response.ok) {
      const error = await response.json()
      throw new Error(error.message || 'Failed to preview summary')
    }
    return response.json()
  },

  // Test endpoints
  async getConfigStatus(): Promise<ConfigStatus> {
    const response = await fetch(`${API_BASE}/test/config`)
    if (!response.ok) throw new Error('Failed to get config status')
    return response.json()
  },

  async testScrape(url: string): Promise<ScrapeTestResult> {
    const response = await fetch(`${API_BASE}/test/scrape`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ url }),
    })
    if (!response.ok) throw new Error('Failed to test scrape')
    return response.json()
  },

  async testAi(url: string): Promise<AiTestResult> {
    const response = await fetch(`${API_BASE}/test/ai`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ url }),
    })
    if (!response.ok) throw new Error('Failed to test AI')
    return response.json()
  },

  async testEmail(email: string): Promise<EmailTestResult> {
    const response = await fetch(`${API_BASE}/test/email`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ email }),
    })
    if (!response.ok) throw new Error('Failed to test email')
    return response.json()
  },

  async testFullPipeline(url: string, email?: string): Promise<FullTestResult> {
    const response = await fetch(`${API_BASE}/test/full`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ url, email }),
    })
    if (!response.ok) throw new Error('Failed to run full test')
    return response.json()
  },

  async previewBrief(topic: string, country: string = 'global'): Promise<PreviewBriefResponse> {
    const response = await fetch(`${API_BASE}/brief/preview`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ topic, country }),
    })
    return response.json()
  },

  async sendBriefToEmail(email: string, subject: string, htmlContent: string): Promise<SendEmailResponse> {
    const response = await fetch(`${API_BASE}/brief/send-email`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ email, subject, htmlContent }),
    })
    return response.json()
  },
}

// Re-export types for convenience
export type {
  PreviewBriefResponse,
  GenerateBriefResponse,
  SendEmailResponse,
  WebsiteBriefResult,
  ConfigStatus,
  ScrapeTestResult,
  AiTestResult,
  EmailTestResult,
  FullTestResult,
} from './types'
