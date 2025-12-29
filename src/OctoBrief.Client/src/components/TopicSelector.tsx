import { POPULAR_TOPICS } from '../constants'

interface TopicSelectorProps {
  selectedTopic: string
  onSelectTopic: (topic: string) => void
}

export function TopicSelector({ selectedTopic, onSelectTopic }: TopicSelectorProps) {
  const isSelected = (topic: string) => selectedTopic.toLowerCase() === topic.toLowerCase()

  return (
    <div>
      <div className="flex items-center gap-2 mb-3">
        <span className="w-6 h-6 rounded-full bg-primary-600 text-white text-xs font-bold flex items-center justify-center">
          1
        </span>
        <label className="font-medium text-gray-900">What topic?</label>
      </div>
      <div className="flex flex-wrap gap-2">
        {POPULAR_TOPICS.map((topic) => (
          <button
            key={topic}
            type="button"
            onClick={() => onSelectTopic(topic)}
            className={`px-3 py-1.5 text-sm rounded-lg transition ${
              isSelected(topic)
                ? 'bg-primary-600 text-white'
                : 'bg-gray-100 text-gray-700 hover:bg-gray-200'
            }`}
          >
            {topic}
          </button>
        ))}
      </div>
    </div>
  )
}
