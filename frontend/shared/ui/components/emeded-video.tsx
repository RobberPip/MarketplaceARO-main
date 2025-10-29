const VideoEmbed = ({ url }) => {
  const getYoutubeId = (url) => {
    const regExp = /(?:youtube\.com.*(?:\?|&)v=|youtu\.be\/)([^&\n?#]+)/;
    const match = url.match(regExp);
    return match ? match[1] : null;
  };

  const getVkEmbedUrl = (url) => {
    const vkComRegex = /vk\.com\/video(-?\d+)_(\d+)(\?.*)?/;
    const vkVideoRuRegex = /vkvideo\.ru\/video(-?\d+)_(\d+)(\?.*)?/;

    let match = url.match(vkComRegex);
    if (match) {
      const oid = match[1];
      const id = match[2];
      const params = match[3] || '';
      return `https://vkvideo.ru/video_ext.php?oid=${oid}&id=${id}${params}`;
    }

    match = url.match(vkVideoRuRegex);
    if (match) {
      const oid = match[1];
      const id = match[2];
      const params = match[3] || '';
      return `https://vkvideo.ru/video_ext.php?oid=${oid}&id=${id}${params}`;
    }

    return null;
  };
  const getRutubeVideoId = (url) => {
    const regExp = /rutube\.ru\/video\/([^\/?\s]+)/;
    const match = url.match(regExp);
    return match ? match[1] : null;
  };

  if (!url) return null;

  // VK Video
  if (url.includes('vk.com') || url.includes('vkvideo.ru')) {
    const src = getVkEmbedUrl(url);
    if (!src) return <div>Invalid VK video URL</div>;
    return (
      <iframe
        src={src}
        width="853"
        height="480"
        style={{ backgroundColor: '#000' }}
        allow="autoplay; encrypted-media; fullscreen; picture-in-picture; screen-wake-lock"
        frameBorder="0"
        allowFullScreen
        title="VK video player"
      />
    );
  }

  // RuTube
  if (url.includes('rutube.ru')) {
    const videoId = getRutubeVideoId(url);
    if (!videoId) return <div>Invalid RuTube URL</div>;
    const src = `https://rutube.ru/play/embed/${videoId}`;
    return <iframe src={src} width="560" height="315" frameBorder="0" allowFullScreen title="RuTube video player" />;
  }

  return <div>Unsupported video URL</div>;
};

export default VideoEmbed;
