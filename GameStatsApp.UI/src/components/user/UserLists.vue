<template>
    <div>
        <div id="toastPlacement" ref="toastcontainer" class="toast-container position-fixed top-0 end-0" style="margin-top:70px;"> 
            <div ref="errortoast" class="toast align-items-center text-white bg-danger border-0" role="alert" aria-live="assertive" aria-atomic="true">
                <div class="d-flex">
                    <div class="toast-body">
                        <span class="msg-text"></span>
                    </div>
                    <button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast" aria-label="Close"></button>
                </div>
            </div>              
            <div ref="successtoast" class="toast align-items-center text-white bg-success border-0" role="alert" aria-live="assertive" aria-atomic="true">
                <div class="d-flex">
                    <div class="toast-body">
                        <span class="msg-text"></span>
                    </div>
                    <button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast" aria-label="Close"></button>
                </div>
            </div>                         
        </div>
        <div class="show-lg d-flex flex-column flex-shrink-0 p-3 position-absolute top-0 start-0 bg-light" style="width: 280px; height: 100vh; margin-top: 63px;">
            <ul class="nav nav-pills flex-column mb-auto">
                <li v-for="(userList, userListIndex) in indexvm.userLists.filter(i => i.defaultListID)" key="userList.id" class="nav-item">
                    <a @click="selectedItemID = userList.id" href="#" class="nav-link" :class="{ 'active' : selectedItemID == userList.id }">
                        <font-awesome-icon :icon="getIconClass(userList.defaultListID)" size="lg" class="me-3"/>
                        <span>{{ userList.name }}</span>
                    </a>
                </li>
                <li v-if="indexvm.userLists.filter(i => !i.defaultListID).length > 0" class="border-top my-3"></li>
                <li v-for="(userList, userListIndex) in indexvm.userLists.filter(i => !i.defaultListID)" key="userList.id" class="nav-item">
                    <a @click="selectedItemID = userList.id" href="#" class="nav-link" :class="{ 'active' : selectedItemID == userList.id }">
                        <span>{{ userList.name }}</span>
                    </a>
                </li>                
            </ul>
        </div>
        <div class="show-md row g-2 justify-content-center">
            <div class="btn-group">
                <button class="btn dropdown-toggle btn-primary d-flex align-items-center" type="button" data-bs-toggle="dropdown" data-bs-display="static" aria-expanded="false">
                    <font-awesome-icon v-if="indexvm.userLists.find(i => i.id == selectedItemID)?.defaultListID" :icon="getIconClass(indexvm.userLists.find(i => i.id == selectedItemID)?.defaultListID)" size="lg"/>
                    <span class="mx-auto">{{ indexvm.userLists.find(i => i.id == selectedItemID)?.name }}</span>
                </button>
                <ul class="dropdown-menu" aria-labelledby="dropdownMenuButton" style="width: 100%;">
                    <li v-for="(userList, userListIndex) in indexvm.userLists" :key="userList.id">
                        <a @click="selectedItemID = userList.id" class="dropdown-item" :class="{ 'active' : selectedItemID == userList.id }" href="#/" data-toggle="pill">{{ userList.name }}</a>
                    </li>
                </ul>
            </div>                
        </div>
        <user-list-games :userid="indexvm.userID" :userlistid="selectedItemID" :userlists="indexvm.userLists" @success="onSuccess" @error="onError" @delete="onDelete"></user-list-games> 
    </div>
</template>
<script>
    import { Toast } from 'bootstrap';

    export default {
        name: "UserLists",
        props: {
            indexvm: Object
        },
        data: function () {
            return {
                userLists: [],
                selectedItemID: 0,
                successMessages: [],
                errorMessages: []
            };
        },       
        watch: {},
        created: function () {
            this.selectedItemID = this.indexvm.userLists[0].id;
        },
        mounted: function() {
            var that = this;
        },
        methods: {
            getIconClass: function (id) {
                var iconClass = '';

                switch (id) {
                    case 1:
                        iconClass = 'fa-solid fa-list';
                        break;
                    case 2:
                        iconClass = 'fa-solid fa-inbox';
                        break;
                    case 3:
                        iconClass = 'fa-solid fa-play';
                        break;
                    case 4:
                        iconClass = 'fa-solid fa-check';
                        break;
                }

                return iconClass;
            },
            onSuccess(successMsg) {
                var that = this;
                var el = that.$refs.successtoast.cloneNode(true);
                el.querySelector('.msg-text').innerHTML = successMsg;
                that.$refs.toastcontainer.appendChild(el);
                new Toast(el).show();
            },
            onError(errorMsg) {
                var that = this;
                var el = that.$refs.errortoast.cloneNode(true);
                el.querySelector('.msg-text').innerHTML = errorMsg;
                that.$refs.toastcontainer.appendChild(el);
                new Toast(el).show();  
            }                                        
        },
    };
</script>






