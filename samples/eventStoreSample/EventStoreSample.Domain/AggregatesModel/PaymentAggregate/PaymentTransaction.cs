﻿using EventStoreSample.Domain.Events;
using EventStoreSample.Domain.Exceptions;
using Galaxy.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventStoreSample.Domain.AggregatesModel.PaymentAggregate
{
    public sealed class PaymentTransaction : AggregateRootEntity<Guid>
    {
        public Money Money { get; private set; }

        public DateTime TransactionDateTime { get; private set; }

        public DateTime? MerchantTransactionDateTime { get; private set; }

        public string Msisdn { get; private set; }

        public string Description { get; private set; }

        public string OrderId { get; private set; }

        public string ResponseCode { get; private set; }

        public string ResponseMessage { get; private set; }

        public int TransactionStatusId { get; private set; }
        public PaymenTransactionStatus PaymenTransactionStatus { get; private set; }

        public int TransactionTypeId { get; private set; }

        public PaymentTransactionType PaymentTransactionType { get; private set; }

        public int? ReferanceTransactionId { get; private set; }

        public PaymentTransaction ReferanceTransaction { get; private set; }

        private PaymentTransaction()
        {
            RegisterEvent<TransactionCreatedDomainEvent>(When);
            RegisterEvent<TransactionAmountChangedDomainEvent>(When);
            RegisterEvent<TransactionStatusChangedDomainEvent>(When);
        }

        private PaymentTransaction(string msisdn, string orderId, DateTime transactionDateTime) : this()
        {
            ApplyEvent(new TransactionCreatedDomainEvent(msisdn, orderId, transactionDateTime));
        }

        public static PaymentTransaction Create(string msisdn, string orderId, DateTime transactionDateTime)
        {
            return new PaymentTransaction(msisdn, orderId, transactionDateTime);
        }

        private void When(TransactionCreatedDomainEvent @event)
        {
            this.Msisdn = !string.IsNullOrWhiteSpace(@event.Msisdn) ? @event.Msisdn
                                                      : throw new ArgumentNullException(nameof(@event.Msisdn));
            this.OrderId = !string.IsNullOrWhiteSpace(@event.OrderId) ? @event.OrderId
                                                     : throw new ArgumentNullException(nameof(@event.OrderId));

            this.TransactionDateTime = @event.TransactionDateTime;

            this.TransactionTypeId = PaymentTransactionType.DirectPaymentType.Id;

            if (DateTime.Now.AddDays(-1) > @event.TransactionDateTime)
            {
                throw new PaymentDomainException($"Invalid transactionDateTime {@event.TransactionDateTime}");
            }
        }

        private void When(TransactionAmountChangedDomainEvent @event)
        {
            this.Money = @event.Money;
        }

        private void When(TransactionStatusChangedDomainEvent @event)
        {
            this.TransactionStatusId = @event.TransactionStatusId;
        }
        
        public PaymentTransaction RefundPaymentTyped()
        {
            this.TransactionTypeId = PaymentTransactionType.RefundPaymentType.Id;
            return this;
        }

        public Money SetMoney(int currencyCode, decimal amount)
        {
            var money = Money.Create(amount, currencyCode);
            this.Money = money;
            return this.Money;
        }

        public void ChangeOrSetAmountTo(Money money)
        {
            // Max Daily Amount. Could get from environment!
            if (money.Amount > 1000)
            {
                throw new PaymentDomainException($"Max daily amount exceed for this transaction {this.Id}");
            }
            // AggregateRoot leads all owned domain events !!!
            ApplyEvent(new TransactionAmountChangedDomainEvent(money));
        }

        public void PaymentStatusSucceded()
        {
            ApplyEvent(new TransactionStatusChangedDomainEvent(PaymenTransactionStatus.SuccessStatus.Id));
        }

        public void PaymentStatusFailed()
        {
            ApplyEvent(new TransactionStatusChangedDomainEvent(PaymenTransactionStatus.FailStatus.Id));
        }
    }
}
