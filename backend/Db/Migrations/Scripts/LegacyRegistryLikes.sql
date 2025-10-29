ALTER TABLE public.software
ADD COLUMN rating NUMERIC(3, 2) DEFAULT 0 CHECK (rating BETWEEN 0 AND 5),
ADD COLUMN review_count INT DEFAULT 0 CHECK (review_count >= 0),
ADD COLUMN like_count INT DEFAULT 0 CHECK (like_count >= 0);
