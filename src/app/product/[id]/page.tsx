'use client';

import { useEffect, useState } from 'react';
import Image from 'next/image';
import { productApi } from '@/lib/api';

interface Product {
  id: number;
  name: string;
  price: number;
  imageUrl: string;
  category: string;
  description: string;
}

export default function ProductDetailPage({ params }: { params: { id: string } }) {
  const [product, setProduct] = useState<Product | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [quantity, setQuantity] = useState(1);

  useEffect(() => {
    const fetchProduct = async () => {
      try {
        setLoading(true);
        const data = await productApi.getProductById(parseInt(params.id));
        setProduct(data);
        setError(null);
      } catch (err) {
        console.error('Error fetching product:', err);
        setError('Failed to load product. Please try again later.');
      } finally {
        setLoading(false);
      }
    };

    fetchProduct();
  }, [params.id]);

  const handleQuantityChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const value = parseInt(e.target.value);
    if (value > 0) {
      setQuantity(value);
    }
  };

  const handleAddToCart = () => {
    // Implement add to cart functionality
    console.log('Add to cart:', product?.id, quantity);
  };

  if (loading) {
    return (
      <div className="container mx-auto px-4 py-8">
        <div className="grid grid-cols-1 md:grid-cols-2 gap-8">
          <div className="bg-gray-200 h-96 animate-pulse"></div>
          <div className="space-y-4">
            <div className="h-8 bg-gray-200 w-3/4 animate-pulse"></div>
            <div className="h-4 bg-gray-200 w-1/4 animate-pulse"></div>
            <div className="h-4 bg-gray-200 w-full animate-pulse"></div>
            <div className="h-4 bg-gray-200 w-full animate-pulse"></div>
            <div className="h-4 bg-gray-200 w-2/3 animate-pulse"></div>
          </div>
        </div>
      </div>
    );
  }

  if (error || !product) {
    return (
      <div className="container mx-auto px-4 py-8">
        <div className="bg-red-100 border border-red-400 text-red-700 px-4 py-3 rounded">
          {error || 'Product not found'}
        </div>
      </div>
    );
  }

  return (
    <div className="container mx-auto px-4 py-8">
      <div className="grid grid-cols-1 md:grid-cols-2 gap-8">
        <div className="relative h-96">
          <Image
            src={product.imageUrl}
            alt={product.name}
            fill
            className="object-cover rounded-lg"
          />
        </div>
        <div>
          <h1 className="text-3xl font-bold mb-2">{product.name}</h1>
          <p className="text-gray-500 mb-4">{product.category}</p>
          <p className="text-2xl font-bold text-primary mb-4">${product.price.toFixed(2)}</p>
          <p className="text-gray-700 mb-6">{product.description}</p>
          
          <div className="mb-6">
            <label htmlFor="quantity" className="block text-sm font-medium text-gray-700 mb-2">
              Quantity
            </label>
            <input
              type="number"
              id="quantity"
              min="1"
              value={quantity}
              onChange={handleQuantityChange}
              className="w-20 px-3 py-2 border border-gray-300 rounded-md"
            />
          </div>
          
          <button
            onClick={handleAddToCart}
            className="w-full bg-primary text-white py-3 px-6 rounded-md hover:bg-primary/90 transition-colors"
          >
            Add to Cart
          </button>
        </div>
      </div>
    </div>
  );
} 