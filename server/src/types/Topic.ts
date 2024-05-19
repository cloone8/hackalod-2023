type Image = {
  url: URL
}

type LoadedTopic = {
  name: string
  description: string
  url: URL
  images: [URL, Image][]
  topics: [URL, Topic][]
}

type Topic = URL | LoadedTopic

function isLoaded(topic: Topic): boolean {
  return 'name' in topic;
}

function asLoadedTopic(topic: Topic): topic is LoadedTopic {
  return isLoaded(topic);
}

function asURL(topic: Topic): topic is URL {
  return !isLoaded(topic);
}
