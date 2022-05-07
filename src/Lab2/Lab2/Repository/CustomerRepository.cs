﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Lab2.Exeptions;
using Lab2.Model;
using Microsoft.Extensions.Configuration;

namespace Lab2.Repository
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly string _storageFileName;
        public CustomerRepository(IConfiguration configuration)
        {
            _storageFileName = configuration.GetValue<string>("CustomersFile");
        }
        private List<Customer> _customers { get; set; }
        public void ReadFromFileCustomers()
        {
            if (!File.Exists(_storageFileName))
            {
                _customers = new List<Customer>();
                return;
            }
            using (var fileReader = new StreamReader(_storageFileName))
            {
                string jsonString = fileReader.ReadToEnd();
                _customers = JsonSerializer.Deserialize<List<Customer>>(jsonString);
            }
            
        }
        public void WriteToFileCustomers()
        {
            string jsonString = JsonSerializer.Serialize(_customers);
            using (var fileWriter = new StreamWriter(_storageFileName))
            {
                fileWriter.Write(jsonString);
            }
        }
        public void AddCustomer(Customer customer)
        {
            if (_customers.Count == 0)
            {
                customer.Id = 1;
            }
            else
            {
                customer.Id = _customers.Max(custom => custom.Id) + 1;
            }
            _customers.Add(customer);           
        }
        public int ReplaceCustomer(int id, Customer customer)
        {
            if (id < 0)
            {
                throw new ArgumentOutOfRangeException();
            }
            var customerIndex = _customers.FindIndex(customer => customer.Id == id);
            if (customerIndex > 0)
            {
                Customer newCustomer = _customers[customerIndex];
                newCustomer.FullName = customer.FullName;
                newCustomer.PhoneNumber = customer.PhoneNumber;
            }
            else
            {
                throw new NotFoundException();
            }
            return id;
        }
        public List<Customer> GetAllCustomers()
        {
            return _customers;
        }
        public Customer GetCustomer(int id)
        {
            if( id < 0)
            {
                throw new ArgumentOutOfRangeException();
            }
            IEnumerable<Customer> customers =  _customers.Where(customer => customer.Id == id);
            if(!customers.Any())
            {
                throw new NotFoundException();
            }
            return customers.Single();
        }
        public int DeleteCustomer(int id)
        {
            if (id < 0)
            {
                throw new ArgumentOutOfRangeException();
            }
            Customer customer = GetCustomer(id);
            if(customer == null)
            {
                throw new NotFoundException();
            }
            _customers.Remove(customer);
            return id;
        }
        public void DeleteAllCustomers()
        {
            _customers.Clear();
        }
    }
}
