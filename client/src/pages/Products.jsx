import { useState, useEffect } from 'react';
import { Plus, Search, Edit2, Trash2, Package } from 'lucide-react';
import api, { getErrorMessage } from '../lib/api';
import Layout from '../components/Layout';

const Products = () => {
  const [products, setProducts] = useState([]);
  const [isLoading, setIsLoading] = useState(true);
  const [searchTerm, setSearchTerm] = useState('');
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [editingProduct, setEditingProduct] = useState(null);
  const [error, setError] = useState('');
  const [successMessage, setSuccessMessage] = useState('');
  const [formData, setFormData] = useState({
    name: '',
    description: '',
    quantity: 0,
    price: 0,
  });

  useEffect(() => {
    fetchProducts();
  }, []);

  const fetchProducts = async () => {
    try {
      setError('');
      const response = await api.get('/products');
      setProducts(response.data);
    } catch (error) {
      const errorMsg = getErrorMessage(error);
      setError(errorMsg);
      console.error('Error fetching products:', errorMsg);
    } finally {
      setIsLoading(false);
    }
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError('');
    setSuccessMessage('');
    
    try {
      if (editingProduct) {
        await api.put(`/products/${editingProduct.id}`, formData);
        setSuccessMessage('Product updated successfully!');
      } else {
        await api.post('/products', formData);
        setSuccessMessage('Product created successfully!');
      }
      fetchProducts();
      closeModal();
      // Clear success message after 3 seconds
      setTimeout(() => setSuccessMessage(''), 3000);
    } catch (error) {
      const errorMsg = getErrorMessage(error);
      setError(errorMsg);
      console.error('Error saving product:', errorMsg);
    }
  };

  const handleDelete = async (id) => {
    if (window.confirm('Are you sure you want to delete this product?')) {
      try {
        setError('');
        setSuccessMessage('');
        await api.delete(`/products/${id}`);
        setSuccessMessage('Product deleted successfully!');
        fetchProducts();
        // Clear success message after 3 seconds
        setTimeout(() => setSuccessMessage(''), 3000);
      } catch (error) {
        const errorMsg = getErrorMessage(error);
        setError(errorMsg);
        console.error('Error deleting product:', errorMsg);
      }
    }
  };

  const openModal = (product = null) => {
    if (product) {
      setEditingProduct(product);
      setFormData({
        name: product.name,
        description: product.description,
        quantity: product.quantity,
        price: product.price,
      });
    } else {
      setEditingProduct(null);
      setFormData({ name: '', description: '', quantity: 0, price: 0 });
    }
    setIsModalOpen(true);
  };

  const closeModal = () => {
    setIsModalOpen(false);
    setEditingProduct(null);
    setFormData({ name: '', description: '', quantity: 0, price: 0 });
  };

  const filteredProducts = products.filter((product) =>
    product.name.toLowerCase().includes(searchTerm.toLowerCase()) ||
    product.description.toLowerCase().includes(searchTerm.toLowerCase())
  );

  return (
    <Layout>
      <div className="space-y-6">
        {/* Error Message */}
        {error && (
          <div className="p-4 bg-red-50 border border-red-200 text-red-700 rounded-lg flex justify-between items-center">
            <span>{error}</span>
            <button onClick={() => setError('')} className="text-red-700 hover:text-red-900 font-bold">
              ×
            </button>
          </div>
        )}

        {/* Success Message */}
        {successMessage && (
          <div className="p-4 bg-green-50 border border-green-200 text-green-700 rounded-lg flex justify-between items-center">
            <span>{successMessage}</span>
            <button onClick={() => setSuccessMessage('')} className="text-green-700 hover:text-green-900 font-bold">
              ×
            </button>
          </div>
        )}

        {/* Header */}
        <div className="flex flex-col sm:flex-row justify-between items-start sm:items-center gap-4">
          <div>
            <h2 className="text-3xl font-bold text-gray-900">Products</h2>
            <p className="text-gray-600 mt-1">Manage your inventory items</p>
          </div>
          <button onClick={() => openModal()} className="btn-primary flex items-center gap-2">
            <Plus size={20} />
            Add Product
          </button>
        </div>

        {/* Search Bar */}
        <div className="card">
          <div className="relative">
            <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 text-gray-400" size={20} />
            <input
              type="text"
              placeholder="Search products..."
              className="input-field pl-10"
              value={searchTerm}
              onChange={(e) => setSearchTerm(e.target.value)}
            />
          </div>
        </div>

        {/* Products Grid */}
        {isLoading ? (
          <div className="flex justify-center items-center py-12">
            <div className="animate-spin rounded-full h-12 w-12 border-t-2 border-b-2 border-primary-600"></div>
          </div>
        ) : filteredProducts.length === 0 ? (
          <div className="card text-center py-12">
            <Package className="mx-auto text-gray-400 mb-4" size={48} />
            <h3 className="text-lg font-medium text-gray-900 mb-2">No products found</h3>
            <p className="text-gray-600">
              {searchTerm ? 'Try adjusting your search' : 'Get started by adding your first product'}
            </p>
          </div>
        ) : (
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
            {filteredProducts.map((product) => (
              <div key={product.id} className="card hover:shadow-lg transition-shadow">
                <div className="flex justify-between items-start mb-4">
                  <div className="flex-1">
                    <h3 className="text-lg font-semibold text-gray-900 mb-1">{product.name}</h3>
                    <p className="text-gray-600 text-sm line-clamp-2">{product.description}</p>
                  </div>
                </div>
                
                <div className="grid grid-cols-2 gap-4 mb-4 py-4 border-t border-b border-gray-100">
                  <div>
                    <p className="text-xs text-gray-500 mb-1">Quantity</p>
                    <p className="text-lg font-semibold text-gray-900">{product.quantity}</p>
                  </div>
                  <div>
                    <p className="text-xs text-gray-500 mb-1">Price</p>
                    <p className="text-lg font-semibold text-primary-600">${product.price.toFixed(2)}</p>
                  </div>
                </div>

                <div className="flex gap-2">
                  <button
                    onClick={() => openModal(product)}
                    className="flex-1 btn-secondary flex items-center justify-center gap-2"
                  >
                    <Edit2 size={16} />
                    Edit
                  </button>
                  <button
                    onClick={() => handleDelete(product.id)}
                    className="flex-1 btn-danger flex items-center justify-center gap-2"
                  >
                    <Trash2 size={16} />
                    Delete
                  </button>
                </div>
              </div>
            ))}
          </div>
        )}
      </div>

      {/* Modal */}
      {isModalOpen && (
        <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center p-4 z-50">
          <div className="bg-white rounded-xl shadow-xl max-w-md w-full p-6">
            <h3 className="text-2xl font-bold text-gray-900 mb-6">
              {editingProduct ? 'Edit Product' : 'Add New Product'}
            </h3>
            <form onSubmit={handleSubmit} className="space-y-4">
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-2">
                  Product Name
                </label>
                <input
                  type="text"
                  required
                  className="input-field"
                  value={formData.name}
                  onChange={(e) => setFormData({ ...formData, name: e.target.value })}
                />
              </div>
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-2">
                  Description
                </label>
                <textarea
                  required
                  rows={3}
                  className="input-field resize-none"
                  value={formData.description}
                  onChange={(e) => setFormData({ ...formData, description: e.target.value })}
                />
              </div>
              <div className="grid grid-cols-2 gap-4">
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-2">
                    Quantity
                  </label>
                  <input
                    type="number"
                    required
                    min="0"
                    className="input-field"
                    value={formData.quantity}
                    onChange={(e) => setFormData({ ...formData, quantity: parseInt(e.target.value) })}
                  />
                </div>
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-2">
                    Price ($)
                  </label>
                  <input
                    type="number"
                    required
                    min="0"
                    step="0.01"
                    className="input-field"
                    value={formData.price}
                    onChange={(e) => setFormData({ ...formData, price: parseFloat(e.target.value) })}
                  />
                </div>
              </div>
              <div className="flex gap-3 pt-4">
                <button type="button" onClick={closeModal} className="flex-1 btn-secondary">
                  Cancel
                </button>
                <button type="submit" className="flex-1 btn-primary">
                  {editingProduct ? 'Update' : 'Create'}
                </button>
              </div>
            </form>
          </div>
        </div>
      )}
    </Layout>
  );
};

export default Products;
