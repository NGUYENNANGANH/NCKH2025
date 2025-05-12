'use client';

import { useEffect, useState } from 'react';
import Image from 'next/image';
import Link from 'next/link';
import { cartApi } from '@/lib/api';

interface CartItem {
  id: number;
  productId: number;
  name: string;
  price: number;
  imageUrl: string;
  quantity: number;
}

export default function CartPage() {
  const [cartItems, setCartItems] = useState<CartItem[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    const fetchCart = async () => {
      try {
        setLoading(true);
        const data = await cartApi.getCart();
        setCartItems(data);
        setError(null);
      } catch (err) {
        console.error('Error fetching cart:', err);
        setError('Failed to load cart. Please try again later.');
      } finally {
        setLoading(false);
      }
    };

    fetchCart();
  }, []);

  const handleQuantityChange = async (itemId: number, newQuantity: number) => {
    if (newQuantity < 1) return;
    
    try {
      await cartApi.updateCartItem(itemId, newQuantity);
      setCartItems(prevItems => 
        prevItems.map(item => 
          item.id === itemId ? { ...item, quantity: newQuantity } : item
        )
      );
    } catch (err) {
      console.error('Error updating cart item:', err);
    }
  };

  const handleRemoveItem = async (itemId: number) => {
    try {
      await cartApi.removeFromCart(itemId);
      setCartItems(prevItems => prevItems.filter(item => item.id !== itemId));
    } catch (err) {
      console.error('Error removing cart item:', err);
    }
  };

  const calculateSubtotal = () => {
    return cartItems.reduce((total, item) => total + item.price * item.quantity, 0);
  };

  if (loading) {
    return (
      <div className="container mx-auto px-4 py-8">
        <h1 className="text-3xl font-bold mb-6">Shopping Cart</h1>
        <div className="animate-pulse">
          {[...Array(3)].map((_, index) => (
            <div key={index} className="flex items-center py-4 border-b">
              <div className="w-24 h-24 bg-gray-200 rounded"></div>
              <div className="ml-4 flex-1">
                <div className="h-4 bg-gray-200 w-1/4 mb-2"></div>
                <div className="h-4 bg-gray-200 w-1/3"></div>
              </div>
            </div>
          ))}
        </div>
      </div>
    );
  }

  if (error) {
    return (
      <div className="container mx-auto px-4 py-8">
        <h1 className="text-3xl font-bold mb-6">Shopping Cart</h1>
        <div className="bg-red-100 border border-red-400 text-red-700 px-4 py-3 rounded">
          {error}
        </div>
      </div>
    );
  }

  if (cartItems.length === 0) {
    return (
      <div className="container mx-auto px-4 py-8">
        <h1 className="text-3xl font-bold mb-6">Shopping Cart</h1>
        <div className="text-center py-12">
          <p className="text-gray-500 mb-4">Your cart is empty</p>
          <Link href="/products" className="bg-primary text-white px-6 py-3 rounded-md hover:bg-primary/90 transition-colors">
            Continue Shopping
          </Link>
        </div>
      </div>
    );
  }

  return (
    <div className="container mx-auto px-4 py-8">
      <h1 className="text-3xl font-bold mb-6">Shopping Cart</h1>
      
      <div className="grid grid-cols-1 lg:grid-cols-3 gap-8">
        <div className="lg:col-span-2">
          {cartItems.map((item) => (
            <div key={item.id} className="flex items-center py-4 border-b">
              <div className="relative w-24 h-24">
                <Image
                  src={item.imageUrl}
                  alt={item.name}
                  fill
                  className="object-cover rounded"
                />
              </div>
              
              <div className="ml-4 flex-1">
                <Link href={`/product/${item.productId}`} className="text-lg font-semibold hover:text-primary">
                  {item.name}
                </Link>
                <p className="text-gray-500">${item.price.toFixed(2)}</p>
                
                <div className="flex items-center mt-2">
                  <button
                    onClick={() => handleQuantityChange(item.id, item.quantity - 1)}
                    className="px-2 py-1 border rounded-l"
                  >
                    -
                  </button>
                  <input
                    type="number"
                    min="1"
                    value={item.quantity}
                    onChange={(e) => handleQuantityChange(item.id, parseInt(e.target.value))}
                    className="w-16 text-center border-t border-b"
                  />
                  <button
                    onClick={() => handleQuantityChange(item.id, item.quantity + 1)}
                    className="px-2 py-1 border rounded-r"
                  >
                    +
                  </button>
                  
                  <button
                    onClick={() => handleRemoveItem(item.id)}
                    className="ml-4 text-red-500 hover:text-red-700"
                  >
                    Remove
                  </button>
                </div>
              </div>
              
              <div className="text-right">
                <p className="font-semibold">${(item.price * item.quantity).toFixed(2)}</p>
              </div>
            </div>
          ))}
        </div>
        
        <div className="bg-gray-50 p-6 rounded-lg h-fit">
          <h2 className="text-xl font-semibold mb-4">Order Summary</h2>
          
          <div className="space-y-2 mb-4">
            <div className="flex justify-between">
              <span>Subtotal</span>
              <span>${calculateSubtotal().toFixed(2)}</span>
            </div>
            <div className="flex justify-between">
              <span>Shipping</span>
              <span>Calculated at checkout</span>
            </div>
            <div className="flex justify-between">
              <span>Tax</span>
              <span>Calculated at checkout</span>
            </div>
          </div>
          
          <div className="border-t pt-4 mb-6">
            <div className="flex justify-between font-bold">
              <span>Total</span>
              <span>${calculateSubtotal().toFixed(2)}</span>
            </div>
          </div>
          
          <Link
            href="/checkout"
            className="block w-full bg-primary text-white text-center py-3 rounded-md hover:bg-primary/90 transition-colors"
          >
            Proceed to Checkout
          </Link>
        </div>
      </div>
    </div>
  );
} 