const { QueueServiceClient } = require("@azure/storage-queue");

(async () => {
    const queueNames = ["queue-connection", "queue-content"];
    const queueServiceClient = QueueServiceClient.fromConnectionString("UseDevelopmentStorage=true");
    let queues = [];
    for await (const queue of queueServiceClient.listQueues()) {
        queues.push(queue.name);
    }
    queueNames.forEach(async queueName => {
        if (queues.includes(queueName)) {
            console.log(`Queue ${queueName} already exists`);
            return;
        } else {
            await queueServiceClient.createQueue(queueName);
            console.log(`Created queue ${queueName} successfully`);
        }
    });
})();