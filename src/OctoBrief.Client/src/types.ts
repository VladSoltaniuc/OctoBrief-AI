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
