import { COUNTRIES } from '../constants'

interface CountrySelectorProps {
  selectedCountry: string
  onSelectCountry: (country: string) => void
}

export function CountrySelector({ selectedCountry, onSelectCountry }: CountrySelectorProps) {
  return (
    <div>
      <div className="flex items-center gap-2 mb-3">
        <span className="w-6 h-6 rounded-full bg-primary-600 text-white text-xs font-bold flex items-center justify-center">
          2
        </span>
        <label className="font-medium text-gray-900">What region?</label>
      </div>
      <div className="grid grid-cols-5 gap-2">
        {COUNTRIES.map((country) => (
          <button
            key={country.value}
            type="button"
            onClick={() => onSelectCountry(country.value)}
            className={`px-2 py-2 text-sm rounded-lg transition text-center ${
              selectedCountry === country.value
                ? 'bg-primary-600 text-white'
                : 'bg-gray-100 text-gray-700 hover:bg-gray-200'
            }`}
          >
            {country.label}
          </button>
        ))}
      </div>
    </div>
  )
}
