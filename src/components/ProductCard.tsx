'use client';

import Image from 'next/image';
import Link from 'next/link';
import { useState } from 'react';

interface ProductCardProps {
  id: number;
  title: string;
  price: number;
  oldPrice?: number;
  image: string;
  rating: number;
  reviews: number;
}

export default function ProductCard({
  id,
  title,
  price,
  oldPrice,
  image,
  rating,
  reviews,
}: ProductCardProps) {
  const [isWishlist, setIsWishlist] = useState(false);

  return (
    <div className="product-card">
      <div className="product-image">
        <Link href={`/product-details/${id}`}>
          <Image
            src={image}
            alt={title}
            width={300}
            height={300}
            className="img-fluid"
          />
        </Link>
        <div className="product-actions">
          <button
            className="btn-action"
            onClick={() => setIsWishlist(!isWishlist)}
            title={isWishlist ? 'Remove from wishlist' : 'Add to wishlist'}
          >
            <i
              className={`bi bi-heart${isWishlist ? '-fill text-danger' : ''}`}
            ></i>
          </button>
          <button className="btn-action" title="Quick view">
            <i className="bi bi-eye"></i>
          </button>
        </div>
      </div>
      <div className="product-info">
        <h3 className="product-title">
          <Link href={`/product-details/${id}`} className="text-decoration-none text-dark">
            {title}
          </Link>
        </h3>
        <div className="product-price">
          <span className="price">${price.toFixed(2)}</span>
          {oldPrice && (
            <span className="price-old">${oldPrice.toFixed(2)}</span>
          )}
        </div>
        <div className="d-flex align-items-center mt-2">
          <div className="text-warning me-2">
            {[...Array(5)].map((_, i) => (
              <i
                key={i}
                className={`bi bi-star${
                  i < Math.floor(rating) ? '-fill' : ''
                }`}
              ></i>
            ))}
          </div>
          <small className="text-muted">({reviews} reviews)</small>
        </div>
        <button className="btn btn-primary w-100 mt-3">
          <i className="bi bi-cart-plus me-2"></i>
          Add to Cart
        </button>
      </div>
    </div>
  );
} 