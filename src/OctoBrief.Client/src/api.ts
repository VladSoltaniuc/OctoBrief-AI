import { WebsiteSummary, ConfigStatus, PreviewBriefResponse, SendEmailResponse,} from './types'
const API_BASE = '/api'
export const api = {
  async triggerMonitoring(id: number): Promise<void> {
    const response = await fetch(`${API_BASE}/preferences/${id}/trigger`, { method: 'POST', })
    if (!response.ok) throw new Error('Failed to trigger monitoring')
  },
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
  async getConfigStatus(): Promise<ConfigStatus> {
    const response = await fetch(`${API_BASE}/test/config`)
    if (!response.ok) throw new Error('Failed to get config status')
    return response.json()
  },
  async previewBrief(topic: string, country: string = 'global'): Promise<PreviewBriefResponse> {
    const response = await fetch(`${API_BASE}/brief/preview`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ topic: topic.toLowerCase(), country: country.toLowerCase() }),
    })
    return response.json()
  },
  async sendBriefToEmail(email: string, subject: string, htmlContent: string): Promise<SendEmailResponse> {
    const response = await fetch(`${API_BASE}/email/send`, { method: 'POST', headers: { 'Content-Type': 'application/json' }, body: JSON.stringify({ email, subject, htmlContent }), })
    return response.json()
  },
}
export type { PreviewBriefResponse, SendEmailResponse, ConfigStatus} from './types'
