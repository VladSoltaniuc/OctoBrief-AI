export enum NotificationFrequency {
  Daily = 0,
  Weekly = 1,
  Monthly = 2,
}

export interface MonitoringPreference {
  id: number
  email: string
  websiteUrl: string
  websiteName: string
  frequency: NotificationFrequency
  isActive: boolean
  createdAt: string
  lastProcessedAt?: string
}

export interface CreatePreferenceDto {
  email: string
  websiteUrl: string
  websiteName: string
  frequency: NotificationFrequency
}

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

// Test API types
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

export interface AiTestResult {
  success: boolean
  error?: string
  message: string
  usingFallback: boolean
  overview?: string
  headlineCount: number
  headlines: HeadlineItem[]
  scrapeElapsedMs: number
  aiElapsedMs: number
}

export interface EmailTestResult {
  success: boolean
  error?: string
  message: string
  hint?: string
  sentTo?: string
  elapsedMs: number
}

export interface FullTestResult {
  success: boolean
  message: string
  steps: TestStep[]
  preview?: { title: string; overview: string; headlines: string[] }
}

export interface TestStep {
  name: string
  status: string
  elapsedMs: number
  details?: string
}

// Brief API types
export interface GenerateBriefResponse {
  success: boolean
  message: string
  subject?: string
  websiteResults?: WebsiteBriefResult[]
  totalHeadlines?: number
  topic?: string
  country?: string
  error?: string
}

export interface PreviewBriefResponse {
  success: boolean
  subject?: string
  htmlContent?: string
  websiteResults?: WebsiteBriefResult[]
  totalHeadlines?: number
  topic?: string
  country?: string
  message?: string
}

export interface WebsiteBriefResult {
  url: string
  websiteName: string
  success: boolean
  headlinesFound: number
  isMajorOutlet?: boolean
  error?: string
}

export interface SendEmailResponse {
  success: boolean
  message: string
}
