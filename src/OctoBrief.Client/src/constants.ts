export const COUNTRIES = [
  { value: 'romania', label: 'Romania' },
  { value: 'usa', label: 'USA' },
  { value: 'uk', label: 'UK' },
  { value: 'canada', label: 'Canada' },
  { value: 'germany', label: 'Germany' },
  { value: 'france', label: 'France' },
  { value: 'italy', label: 'Italy' },
  { value: 'spain', label: 'Spain' },
  { value: 'poland', label: 'Poland' },
] as const

export const POPULAR_TOPICS = [
  'Technology',
  'Science',
  'Sports',
  'Media',
  'Health',
  'Climate',
  'Politics',
  'Crypto',
  'Gaming',
] as const

export type Country = (typeof COUNTRIES)[number]
export type TopicName = (typeof POPULAR_TOPICS)[number]
