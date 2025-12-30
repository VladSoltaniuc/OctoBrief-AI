export interface HeadlineItem {
  title: string
  summary: string
  sourceUrl: string
}
export interface WebsiteSummary {
  id: number
  websiteName: string
  websiteUrl: string
  summary: string
  headlines: HeadlineItem[]
  createdAt: string
}
export interface ConfigStatus {
  openAiConfigured: boolean
  emailConfigured: boolean
  smtpHost: string
  smtpPort: string
}
export interface ScrapeTestResult {
  success: boolean
  error?: string
  message: string
  title?: string
  headlinesFound: number
  headlines: string[]
  contentLength: number
  contentPreview?: string
  elapsedMs: number
}
export interface PreviewBriefResponse {
  success: boolean
  subject?: string
  htmlContent?: string
  sources: number
  totalHeadlines?: number
  topic?: string
  country?: string
  message?: string
}
export interface SendEmailResponse {
  success: boolean
  message: string
}