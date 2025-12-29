import { COUNTRIES } from '../constants'

interface LoadingStateProps {
  topic: string
  country: string
  loadingStep: string
}

export function LoadingState({ topic, country, loadingStep }: LoadingStateProps) {
  const countryLabel = COUNTRIES.find((c) => c.value === country)?.label || 'Global'

  return (
    <div className="max-w-xl mx-auto">
      <div className="bg-white rounded-2xl shadow-lg p-8">
        <div className="text-center">
          <h3 className="text-lg font-semibold text-gray-900 mb-1">
            Searching {topic} in {countryLabel}
          </h3>
          <p className="text-gray-500 text-sm">
            {loadingStep || 'This may take 15-30 seconds...'}
          </p>
        </div>
      </div>
    </div>
  )
}
